using DslExperiment.Domain.ViewModels;
using System.Text.Json.Serialization;

namespace DslExperiment.Kafka.Producers.Models
{
    public class TransactionCategoryKafkaModel
    {
        public TransactionCategoryKafkaModel(TransactionCategoryViewModel transactionCategoryViewModel)
        {
            TransactionId = transactionCategoryViewModel.TransactionId;
            Category = transactionCategoryViewModel.Category;
        }

        [JsonPropertyName("transactionid")]
        public string TransactionId { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}