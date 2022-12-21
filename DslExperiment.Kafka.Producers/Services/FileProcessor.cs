using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using CsvHelper;
using CsvHelper.Configuration;
using DslExperiment.Domain.ViewModels;
using DslExperiment.Kafka.Producers.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DslExperiment.Kafka.Producers.Services
{
    public class FileProcessor : IFileProcessor
    {
        private string BlobConnectionString = "DefaultEndpointsProtocol=https;AccountName=transactioncategories;AccountKey=u5eIKXHLOyqUhcZPs2868XiIUO/JRmEditALdj/N5sTc9hLIVU0gIAF5s57oOjBTTff+NoeKYXy4+ASt7YdcUQ==;EndpointSuffix=core.windows.net";

        private readonly ITransactionCategoryEventProducer _transactionCategoryEventProducer;

        public FileProcessor(ITransactionCategoryEventProducer transactionCategoryEventProducer)
        {
            _transactionCategoryEventProducer = transactionCategoryEventProducer;
        }

        public int Parse(Stream input, ILogger logger)
        {
            var batchSize = 64;
            var recordCount = 0;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = "|",
                Encoding = Encoding.UTF8,
                TrimOptions = TrimOptions.Trim
            };

            logger.LogInformation($"Parsed batchSize {batchSize}..");

            using (var reader = new StreamReader(input))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<TransactionCategoryMap>();
                var records = csv.GetRecords<TransactionCategoryViewModel>().ToList();

                if (batchSize > 1)
                {
                    logger.LogInformation($"Sending to kakfa by batch of {batchSize}..");
                    IEnumerable<TransactionCategoryViewModel[]> accountRelationships = records.Chunk(batchSize);
                    foreach (var batch in accountRelationships)
                    {
                        TransactionCategoryKafkaModel[] kafkaModels = Array.ConvertAll(batch, new Converter<TransactionCategoryViewModel, TransactionCategoryKafkaModel>(TransactionCategoryToKafkaConverter.ToKafkaModel));
                        _transactionCategoryEventProducer.PublishToStream(kafkaModels, logger);
                        recordCount = recordCount + batch.Count();
                    }
                }
                else
                {
                    logger.LogInformation("Sending to kakfa one by one..");
                    foreach (var record in records)
                    {
                        TransactionCategoryKafkaModel kafkaModel = new TransactionCategoryKafkaModel(record);
                        _transactionCategoryEventProducer.PublishToStream(kafkaModel, logger);
                        recordCount += 1;
                    }
                }
            }

            return recordCount;
        }

        public void CleanUp(string uri, ILogger logger)
        {
            try
            {
                string filename = Path.GetFileName(uri);
                logger.LogInformation($"Filename to clean up: {filename} from uri ${uri}");

                BlobClient sourceClient = new BlobClient(BlobConnectionString, GlobalConstants.STORAGE_CONTAINER_IN, filename);
                BlobClient targetClient = new BlobClient(BlobConnectionString, GlobalConstants.STORAGE_CONTAINER_PROCESSED, filename + GlobalConstants.PROCESSED_FILE_EXTENSION + DateTime.Now.Ticks);
                CopyFromUriOperation ops = targetClient.StartCopyFromUri(GetSharedAccessUri(sourceClient.Name, sourceClient));
                sourceClient.DeleteIfExists();
                logger.LogInformation($"Filename to clean up: {filename} from uri ${uri} deleted.");

            }
            catch (Exception e)
            {
                logger.LogError($"Exception ocurred while cleaning up file: {e.Message}");
                throw;
            }
        }

        private Uri GetSharedAccessUri(string blobName, BlobClient blobClient)
        {
            DateTimeOffset expiredOn = DateTimeOffset.UtcNow.AddHours(2);
            Uri sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, expiredOn);

            return sasUri;
        }
    }
}