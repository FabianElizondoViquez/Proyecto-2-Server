using Entities;
using StoreDataManager;
using System.Text.RegularExpressions;

namespace QueryProcessor.Operations
{
    internal class CreateIndex
    {
        internal OperationStatus Execute(string indexDefinition)
        {
            var match = Regex.Match(indexDefinition, @"(\w+)\s+ON\s+(\w+)\((\w+)\)\s+OF\s+TYPE\s+(BTREE|BST)");
            if (!match.Success)
            {
                return OperationStatus.Error;
            }

            var indexName = match.Groups[1].Value;
            var tableName = match.Groups[2].Value;
            var columnName = match.Groups[3].Value;
            var indexType = match.Groups[4].Value;

            return Store.GetInstance().CreateIndex(indexName, tableName, columnName, indexType);
        }
    }
}