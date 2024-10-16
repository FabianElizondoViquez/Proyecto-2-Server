using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using StoreDataManager;
using System.Text.RegularExpressions;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        public OperationStatus Execute(string query)
        {
            var match = Regex.Match(query, @"INSERT\s+INTO\s+(\w+)\s+VALUES\s*\((.+)\)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return OperationStatus.Error;
            }

            var tableName = match.Groups[1].Value;
            var values = match.Groups[2].Value.Split(',');

            return Store.GetInstance().InsertIntoTable(tableName, values);
        }
    }
}
