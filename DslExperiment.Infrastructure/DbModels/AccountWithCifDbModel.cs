namespace DslExperiment.Infrastructure.DbModels
{
    public class AccountWithCifDbModel
    {
        public Guid CifId { get; set; }

        public string AccountNumber { get; set; }

        public string AccountType { get; set; }
    }
}