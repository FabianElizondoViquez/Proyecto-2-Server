using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class SetDatabase
    {
        internal OperationStatus Execute(string databaseName)
        {
            return Store.GetInstance().ValidateDatabaseExists(databaseName);
        }
    }
}