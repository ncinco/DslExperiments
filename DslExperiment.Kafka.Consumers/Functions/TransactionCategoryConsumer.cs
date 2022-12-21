using Cassandra.Mapping;
using DslExperiment.Domain.ViewModels;
using DslExperiment.Infrastructure.DbModels;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DslExperiment.Kafka.Consumers.Functions
{
    public class TransactionCategoryConsumer
    {
        private readonly IMapper _mapper;
        private ILogger _logger;

        public TransactionCategoryConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        [FunctionName("TransactionCategoryConsumer")]
        public async Task Run([KafkaTrigger("pkc-4n66v.australiaeast.azure.confluent.cloud:9092",
            "dsl-transaction-category",
            Username = "MJSR5BSO4BT5CBY7",
            Password = "kao8mKdbviSmGPGXnt6vNs7nifcbXZy44LWrh7CL18K6hK6UuGGnYKVsW8PykTkA",
            Protocol = BrokerProtocol.SaslSsl,
            AuthenticationMode = BrokerAuthenticationMode.Plain,
            ConsumerGroup = "$Default")] KafkaEventData<string> eventItem,
            ILogger logger)
        {

            _logger = logger;

            try
            {
                await EnrichTransactionByAccount(eventItem);

                _logger.LogInformation($"C# Kafka trigger function processed a message: {eventItem.Value}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());

                // for re-processing
                throw;
            }
        }

        private async Task EnrichTransactionByAccount(KafkaEventData<string> eventItem)
        {
            var transactionCategoryViewModel = JsonConvert.DeserializeObject<TransactionCategoryViewModel>(eventItem.Value);

            var transactionWithCif = await _mapper
                .SingleOrDefaultAsync<TransactionAccountWithCifDbModel>("WHERE transactionId = ?", transactionCategoryViewModel.TransactionId);

            // nothing on db, do nothing
            if (transactionWithCif != null)
            {
                // dont do update if category is just the same
                if (transactionCategoryViewModel.Category != transactionWithCif.Category)
                {
                    var batch = _mapper.CreateBatch(Cassandra.BatchType.Logged);

                    batch.Update<TransactionAccountWithCifDbModel>("SET category = ? WHERE accountnumber = ? AND transactionid = ? AND transactiondate = ?", transactionCategoryViewModel.Category, transactionWithCif.AccountNumber, transactionWithCif.TransactionId, transactionWithCif.TransactionDate);
                    batch.Update<TransactionsByAccountDbModel>("SET category = ? WHERE accountnumber = ? AND transactionid = ? AND transactiondate = ?", transactionCategoryViewModel.Category, transactionWithCif.AccountNumber, transactionWithCif.TransactionId, transactionWithCif.TransactionDate);

                    await _mapper.ExecuteAsync(batch);

                    _logger.LogInformation($"Transaction category for transaction with transaction id {transactionWithCif.TransactionId} was updated.");
                }
            }
        }
    }
}