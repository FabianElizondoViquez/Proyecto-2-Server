using Entities;
using StoreDataManager;
using System.Text.RegularExpressions;

namespace QueryProcessor.Operations
{
    internal class Update
    {
        public OperationStatus Execute(string query)
        {
            var match = Regex.Match(query, @"UPDATE\s+(\w+)\s+SET\s+(\w+)\s*=\s*(.+?)(?:\s+WHERE\s+(.*))?", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return OperationStatus.Error;
            }

            var tableName = match.Groups[1].Value;
            var columnName = match.Groups[2].Value;
            var newValue = match.Groups[3].Value;
            var whereClause = match.Groups[4].Value;

            return Store.GetInstance().UpdateTable(tableName, columnName, newValue, whereClause);
        }
    }
}