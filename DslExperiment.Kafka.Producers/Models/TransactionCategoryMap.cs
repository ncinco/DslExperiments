using CsvHelper.Configuration;
using DslExperiment.Domain.ViewModels;

namespace DslExperiment.Kafka.Producers.Models
{
    public sealed class TransactionCategoryMap : ClassMap<TransactionCategoryViewModel>
    {
        public TransactionCategoryMap()
        {
            Map(m => m.TransactionId).Name("transactionId");
            Map(m => m.Category).Name("category");
        }
    }
}