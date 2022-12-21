using DslExperiment.Kafka.Producers.Models;
using Microsoft.Extensions.Logging;

namespace DslExperiment.Kafka.Producers.Services
{
    public interface ITransactionCategoryEventProducer
    {
        public void PublishToStream(TransactionCategoryKafkaModel kafkaModel, ILogger logger);
        public void PublishToStream(TransactionCategoryKafkaModel[] kakfkaModels, ILogger logger);
    }
}