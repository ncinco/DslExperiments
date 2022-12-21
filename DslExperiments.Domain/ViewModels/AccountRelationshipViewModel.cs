using System.Text.Json.Serialization;

namespace DslExperiment.ViewModels
{
    public class AccountRelationshipViewModel
    {
        [JsonPropertyName("tokenisedCif")]
        public Guid TokenisedCif { get; set; }

        [JsonPropertyName("accountRelationship")]
        public List<AccountRelationshipDetailViewModel> AccountRelationshipDetail { get; set; }
    }

    public class AccountRelationshipDetailViewModel
    {
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}