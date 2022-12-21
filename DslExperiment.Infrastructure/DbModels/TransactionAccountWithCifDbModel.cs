namespace DslExperiment.Infrastructure.DbModels
{
    public class TransactionAccountWithCifDbModel
    {
        public string AccountNumber { get; set; }

        public decimal Amount { get; set; }

        public string Category { get; set; }

        public string[] CifId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Particular { get; set; }

        public DateTimeOffset ProccessedDate { get; set; }

        public string Reference { get; set; }

        public string Status { get; set; }

        public DateTimeOffset TransactionDate { get; set; }

        public string TransactionId { get; set; }
    }
}