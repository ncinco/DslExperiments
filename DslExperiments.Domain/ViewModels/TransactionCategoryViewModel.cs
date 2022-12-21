using CsvHelper.Configuration.Attributes;
using System.Text.Json.Serialization;

namespace DslExperiment.Domain.ViewModels
{
    [Delimiter("|")]
    public class TransactionCategoryViewModel
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}