using Entities;
using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace QueryProcessor.Operations
{
    internal class Select
    {
        public OperationStatus Execute(string query)
        {
            var match = Regex.Match(query, @"SELECT\s+(.*)\s+FROM\s+(\w+)(?:\s+WHERE\s+(.*?))?(?:\s+ORDER\s+BY\s+(\w+)\s+(asc|desc))?", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return OperationStatus.Error;
            }

            var columns = match.Groups[1].Value;
            var tableName = match.Groups[2].Value;
            var whereClause = match.Groups[3].Value;
            var orderByColumn = match.Groups[4].Value;
            var orderDirection = match.Groups[5].Value;

            return Store.GetInstance().SelectFromTable(tableName, columns, whereClause, orderByColumn, orderDirection);
        }
    }
}
