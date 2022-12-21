using Cassandra.Mapping;
using DslExperiment.Infrastructure.DbModels;

namespace DslExperiment.Infrastructure
{
    public class DomainMappings : Mappings
    {
        // to be replaced using config
        private const string KeySpace = "asb";
        private const string PostFix = "_v2";

        public DomainMappings()
        {
            For<AccountWithCifDbModel>()
                .KeyspaceName(KeySpace)
                .TableName($"account_with_cif{PostFix}")
                .PartitionKey(u => u.CifId)
                .Column(u => u.CifId, cm => cm.WithName("cifid"))
                .Column(u => u.AccountNumber, cm => cm.WithName("accountnumber"))
                .Column(u => u.AccountType, cm => cm.WithName("accounttype"));

            For<TransactionsByAccountDbModel>()
                .KeyspaceName(KeySpace)
                .TableName($"transactions_by_account{PostFix}")
                .PartitionKey(u => u.AccountNumber)
                .ClusteringKey(u => u.TransactionDate)
                .Column(u => u.AccountNumber, cm => cm.WithName("accountnumber"))
                .Column(u => u.TransactionId, cm => cm.WithName("transactionid"))
                .Column(u => u.TransactionDate, cm => cm.WithName("transactiondate"))
                .Column(u => u.Amount, cm => cm.WithName("amount"))
                .Column(u => u.Category, cm => cm.WithName("category"))
                .Column(u => u.Code, cm => cm.WithName("code"))
                .Column(u => u.Description, cm => cm.WithName("description"))
                .Column(u => u.Particular, cm => cm.WithName("particular"))
                .Column(u => u.ProccessedDate, cm => cm.WithName("proccesseddate"))
                .Column(u => u.Reference, cm => cm.WithName("reference"))
                .Column(u => u.Status, cm => cm.WithName("status"));

            For<TransactionAccountWithCifDbModel>()
                .KeyspaceName(KeySpace)
                .TableName($"transactions_account_with_cif_view{PostFix}")
                .PartitionKey(u => u.AccountNumber)
                .Column(u => u.AccountNumber, cm => cm.WithName("accountnumber"))
                .Column(u => u.Amount, cm => cm.WithName("amount"))
                .Column(u => u.Category, cm => cm.WithName("category"))
                .Column(u => u.CifId, cm => cm.WithName("cifid"))
                .Column(u => u.Code, cm => cm.WithName("code"))
                .Column(u => u.Description, cm => cm.WithName("description"))
                .Column(u => u.Particular, cm => cm.WithName("particular"))
                .Column(u => u.ProccessedDate, cm => cm.WithName("proccesseddate"))
                .Column(u => u.Reference, cm => cm.WithName("reference"))
                .Column(u => u.Status, cm => cm.WithName("status"))
                .Column(u => u.TransactionDate, cm => cm.WithName("transactiondate"))
                .Column(u => u.TransactionId, cm => cm.WithName("transactionid"));
        }
    }
}