using Cassandra.Mapping;
using DslExperiment.Infrastructure.DbModels;
using DslExperiment.TransactionConsumer;
using DslExperiment.ViewModels;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslExperiment.Kafka.Consumers.Functions
{
    public class AccountRelationshipConsumer
    {
        private readonly IMapper _mapper;
        private ILogger _logger;

        public AccountRelationshipConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        [FunctionName("AccountRelationshipConsumer")]
        public async Task Run([KafkaTrigger("pkc-4n66v.australiaeast.azure.confluent.cloud:9092",
            "dsl-account-relationship",
            Username = "MJSR5BSO4BT5CBY7",
            Password = "kao8mKdbviSmGPGXnt6vNs7nifcbXZy44LWrh7CL18K6hK6UuGGnYKVsW8PykTkA",
            Protocol = BrokerProtocol.SaslSsl,
            AuthenticationMode = BrokerAuthenticationMode.Plain,
            ConsumerGroup = "$Default")] KafkaEventData<string>[] eventItems,
            ILogger logger)
        {
            _logger = logger;

            try
            {
                await EnrichAccountWithCif(eventItems);

                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"[{string.Join(",", eventItems.Select(item => item.Value))}]");
                _logger.LogInformation($"C# Kafka trigger function processed a message: {stringBuilder}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
            }
        }

        private async Task EnrichAccountWithCif(KafkaEventData<string>[] eventItems)
        {
            var accountRelationshipViewModels = eventItems.Select(x => JsonConvert.DeserializeObject<AccountRelationshipViewModel>(x.Value)).ToList();

            foreach (var accountRelationshipViewModel in accountRelationshipViewModels)
            {
                var batch = _mapper.CreateBatch(Cassandra.BatchType.Logged);

                // clean up by cifid
                batch.Delete<AccountWithCifDbModel>("WHERE cifid = ?", accountRelationshipViewModel.TokenisedCif);

                // add new entries
                foreach (var item in accountRelationshipViewModel.AccountRelationshipDetail)
                {
                    batch.InsertIfNotExists(item.ToDbModel(accountRelationshipViewModel.TokenisedCif));
                }

                await _mapper.ExecuteAsync(batch);
            }
        }
    }
}