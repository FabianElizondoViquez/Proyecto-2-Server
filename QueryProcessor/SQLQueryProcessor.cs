using Entities;
using QueryProcessor.Exceptions;
using QueryProcessor.Operations;
using StoreDataManager;

namespace QueryProcessor
{
    public class SQLQueryProcessor
    {
        public static OperationStatus Execute(string sentence)
        {
            if (sentence.StartsWith("CREATE DATABASE"))
            {
                var databaseName = sentence.Substring("CREATE DATABASE".Length).Trim();
                return new CreateDatabase().Execute(databaseName);
            }
            if (sentence.StartsWith("SET DATABASE"))
            {
                var databaseName = sentence.Substring("SET DATABASE".Length).Trim();
                return new SetDatabase().Execute(databaseName);
            }
            if (sentence.StartsWith("CREATE TABLE"))
            {
                var tableDefinition = sentence.Substring("CREATE TABLE".Length).Trim();
                return new CreateTable().Execute(tableDefinition);
            }
            if (sentence.StartsWith("DROP TABLE"))
            {
                var tableName = sentence.Substring("DROP TABLE".Length).Trim();
                return new DropTable().Execute(tableName);
            }
            if (sentence.StartsWith("CREATE INDEX"))
            {
                var indexDefinition = sentence.Substring("CREATE INDEX".Length).Trim();
                return new CreateIndex().Execute(indexDefinition);
            }
            if (sentence.StartsWith("SELECT"))
            {
                return new Select().Execute(sentence);
            }
            if (sentence.StartsWith("UPDATE"))
            {
                return new Update().Execute(sentence);
            }
            if (sentence.StartsWith("DELETE"))
            {
                return new Delete().Execute(sentence);
            }
            if (sentence.StartsWith("INSERT INTO"))
            {
                return new Insert().Execute(sentence);
            }
            else
            {
                throw new UnknownSQLSentenceException();
            }
        }
    }
}
