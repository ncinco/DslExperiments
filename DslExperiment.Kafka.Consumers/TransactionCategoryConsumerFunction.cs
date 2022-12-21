using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;

namespace DslExperiment.Kafka.Consumers
{
    public class TransactionCategoryConsumerFunction
    {
        [FunctionName("TransactionCategoryConsumerFunction")]
        public void Run([KafkaTrigger("pkc-4n66v.australiaeast.azure.confluent.cloud:9092",
            "dsl-transaction-category",
            Username = "MJSR5BSO4BT5CBY7",
            Password = "kao8mKdbviSmGPGXnt6vNs7nifcbXZy44LWrh7CL18K6hK6UuGGnYKVsW8PykTkA",
            Protocol = BrokerProtocol.SaslSsl,
            AuthenticationMode = BrokerAuthenticationMode.Plain,
            ConsumerGroup = "$Default")] KafkaEventData<string> eventItem,
            ILogger log)
        {
            log.LogInformation($"C# Kafka trigger function processed a message: {eventItem.Value}");
        }
    }
}