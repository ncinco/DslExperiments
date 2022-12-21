using DslExperiment.Domain.ViewModels;

namespace DslExperiment.Kafka.Producers.Models
{
    public class TransactionCategoryToKafkaConverter
    {
        public static TransactionCategoryKafkaModel ToKafkaModel(TransactionCategoryViewModel accountRelationship)
        {
            return new TransactionCategoryKafkaModel(accountRelationship);
        }
    }
}