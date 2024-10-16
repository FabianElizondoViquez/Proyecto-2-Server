using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute(string tableDefinition)
        {
            return Store.GetInstance().CreateTable("nombreDeLaTabla", tableDefinition);
        }
    }
}
