using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateDatabase
    {
        internal OperationStatus Execute(string databaseName)
        {
            return Store.GetInstance().CreateDatabase(databaseName);
        }
    }
}