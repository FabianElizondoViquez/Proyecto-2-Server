using Entities;
using StoreDataManager;
using System.Text.RegularExpressions;

namespace QueryProcessor.Operations
{
    internal class Delete
    {
        public OperationStatus Execute(string query)
        {
            var match = Regex.Match(query, @"DELETE\s+FROM\s+(\w+)(?:\s+WHERE\s+(.*))?", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return OperationStatus.Error;
            }

            var tableName = match.Groups[1].Value;
            var whereClause = match.Groups[2].Value;

            return Store.GetInstance().DeleteFromTable(tableName, whereClause);
        }
    }
}