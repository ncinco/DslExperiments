using System.Text.Json.Serialization;

namespace DslExperiment.ViewModels
{
    public class TransactionsViewModel
    {
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("transactionDate")]
        public DateTimeOffset TransactionDate { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("particulars")]
        public string Particular { get; set; }

        [JsonPropertyName("processedDate")]
        public DateTimeOffset ProccessedDate { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }        
    }
}
