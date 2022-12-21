using DslExperiment.Infrastructure.DbModels;
using DslExperiment.ViewModels;
using System;

namespace DslExperiment.TransactionConsumer
{
    public static class MappingHelper
    {
        public static AccountWithCifDbModel ToDbModel(this AccountRelationshipDetailViewModel model, Guid tokenisedCif)
        {
            return new AccountWithCifDbModel
            {
                AccountNumber = model.AccountNumber,
                AccountType = model.Type,
                CifId = tokenisedCif
            };
        }
    }
}