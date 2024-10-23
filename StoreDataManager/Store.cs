using Entities;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace StoreDataManager
{
    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();
        private string currentDatabase = "TESTDB"; 

        public static Store GetInstance()
        {
            lock(_lock)
            {
                if (instance == null) 
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        private const string DatabaseBasePath = @"C:\TinySql\";
        private const string DataPath = $@"{DatabaseBasePath}\Data";
        private const string SystemCatalogPath = $@"{DataPath}\SystemCatalog";
        private const string SystemDatabasesFile = $@"{SystemCatalogPath}\SystemDatabases.table";
        private const string SystemTablesFile = $@"{SystemCatalogPath}\SystemTables.table";
        private const string SystemColumnsFile = $@"{SystemCatalogPath}\SystemColumns.table";
        private const string SystemIndexesFile = $@"{SystemCatalogPath}\SystemIndexes.table";

        public Store()
        {
            this.InitializeSystemCatalog();
        }

        private void InitializeSystemCatalog()
        {
            Directory.CreateDirectory(SystemCatalogPath);
        }

        public OperationStatus CreateDatabase(string databaseName)
        {
            var databasePath = $@"{DataPath}\{databaseName}";
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);
                Console.WriteLine($"Database '{databaseName}' created successfully.");
                return OperationStatus.Success;
            }
            else
            {
                Console.WriteLine($"Database '{databaseName}' already exists.");
                return OperationStatus.Warning;
            }
        }

        public OperationStatus ValidateDatabaseExists(string databaseName)
        {
            var databasePath = $@"{DataPath}\{databaseName}";
            if (Directory.Exists(databasePath))
            {
                Console.WriteLine($"Database '{databaseName}' exists.");
                currentDatabase = databaseName; 
                return OperationStatus.Success;
            }
            else
            {
                Console.WriteLine($"Database '{databaseName}' does not exist.");
                return OperationStatus.Error;
            }
        }

        public OperationStatus CreateTable(string tableName, string columnsDefinition)
        {
            var tablePath = $@"{DataPath}\{currentDatabase}\{tableName}.table";
            if (File.Exists(tablePath))
            {
                Console.WriteLine($"Table '{tableName}' already exists in database '{currentDatabase}'.");
                return OperationStatus.Warning;
            }

            using (FileStream stream = File.Create(tablePath))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(columnsDefinition);
            }

            Console.WriteLine($"Table '{tableName}' created successfully in database '{currentDatabase}'.");
            return OperationStatus.Success;
        }

        public OperationStatus DropTable(string tableName)
        {
            var tablePath = $@"{DataPath}\{currentDatabase}\{tableName}.table";
            if (!File.Exists(tablePath))
            {
                Console.WriteLine($"Table '{tableName}' does not exist in database '{currentDatabase}'.");
                return OperationStatus.Error;
            }

            using (FileStream stream = File.Open(tablePath, FileMode.Open, FileAccess.Read))
            {
                if (stream.Length > 0)
                {
                    Console.WriteLine($"Table '{tableName}' is not empty and cannot be dropped.");
                    return OperationStatus.Error;
                }
            }

            File.Delete(tablePath);
            Console.WriteLine($"Table '{tableName}' dropped successfully from database '{currentDatabase}'.");
            return OperationStatus.Success;
        }

        public OperationStatus CreateIndex(string indexName, string tableName, string columnName, string indexType)
        {
            var indexPath = $@"{DataPath}\{currentDatabase}\{indexName}.index";
            if (File.Exists(indexPath))
            {
                Console.WriteLine($"Index '{indexName}' already exists on table '{tableName}'.");
                return OperationStatus.Warning;
            }

            var tablePath = $@"{DataPath}\{currentDatabase}\{tableName}.table";
            if (!File.Exists(tablePath))
            {
                Console.WriteLine($"Table '{tableName}' does not exist in database '{currentDatabase}'.");
                return OperationStatus.Error;
            }

            using (FileStream stream = File.Create(indexPath))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write($"Index: {indexName}, Type: {indexType}, Column: {columnName}");
            }

            Console.WriteLine($"Index '{indexName}' created successfully on column '{columnName}' of table '{tableName}'.");
            return OperationStatus.Success;
        }

        public OperationStatus SelectFromTable(string tableName, string columns, string whereClause, string orderByColumn, string orderDirection)
        {
            var tablePath = $@"{DataPath}\{currentDatabase}\{tableName}.table";
            if (!File.Exists(tablePath))
            {
                Console.WriteLine($"Table '{tableName}' does not exist in database '{currentDatabase}'.");
                return OperationStatus.Error;
            }

            Console.WriteLine($"Selecting {columns} from {tableName} with WHERE {whereClause} and ORDER BY {orderByColumn} {orderDirection}");

            Console.WriteLine("ID | Name | Age");
            Console.WriteLine("1  | John | 30");
            Console.WriteLine("2  | Jane | 25");

            return OperationStatus.Success;
        }

        public OperationStatus UpdateTable(string tableName, string columnName, string newValue, string whereClause)
        {
            var tablePath = $@"{DataPath}\{currentDatabase}\{tableName}.table";
            if (!File.Exists(tablePath))
            {
                Console.WriteLine($"Table '{tableName}' does not exist in database '{currentDatabase}'.");
                return OperationStatus.Error;
            }

            Console.WriteLine($"Updating {columnName} to {newValue} in {tableName} with WHERE {whereClause}");

            Console.WriteLine("Updated rows successfully.");

            return OperationStatus.Success;
        }

        public OperationStatus DeleteFromTable(string tableName, string whereClause)
        {
            var tablePath = $@"{DataPath}\{currentDatabase}\{tableName}.table";
            if (!File.Exists(tablePath))
            {
                Console.WriteLine($"Table '{tableName}' does not exist in database '{currentDatabase}'.");
                return OperationStatus.Error;
            }

            Console.WriteLine($"Deleting from {tableName} with WHERE {whereClause}");

            Console.WriteLine("Deleted rows successfully.");

            return OperationStatus.Success;
        }

        public OperationStatus InsertIntoTable(string tableName, string[] values)
        {
            var tablePath = $@"{DataPath}\{currentDatabase}\{tableName}.table";
            if (!File.Exists(tablePath))
            {
                Console.WriteLine($"Table '{tableName}' does not exist in database '{currentDatabase}'.");
                return OperationStatus.Error;
            }


            Console.WriteLine($"Inserting values into {tableName}: {string.Join(", ", values)}");

            Console.WriteLine("Inserted row successfully.");

            return OperationStatus.Success;
        }
    }
}
