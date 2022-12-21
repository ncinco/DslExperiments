using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using DslExperiment.Kafka.Producers.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;

namespace DslExperiment.Kafka.Producers.Functions
{
    public class TransactionCategoryProducer
    {
        private readonly IFileProcessor _fileProcessor;

        public TransactionCategoryProducer(IFileProcessor fileProcessor)
        {
            _fileProcessor = fileProcessor;
        }

        [FunctionName("TransactionCagetoryProducer")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent,
            [Blob("{data.url}", FileAccess.Read)] Stream input,
            ILogger logger)
        {
            try
            {
                var recordCount = _fileProcessor.Parse(input, logger);

                logger.LogInformation($"C# BlobStorage trigger function processed a message: {recordCount}");

                StorageBlobCreatedEventData createdEvent = JsonSerializer.Deserialize<StorageBlobCreatedEventData>(eventGridEvent.Data);

                _fileProcessor.CleanUp(createdEvent.Url, logger);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());

                // for re-processing
                throw;
            }
        }
    }
}