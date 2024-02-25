// DynamicEntity.cs

namespace MyApplication.Data
{
    // public class DynamicEntity1
    // {
    //     public Dictionary<string, object> properties = new Dictionary<string, object>();

    //     private IDatabaseProvider databaseProvider;

    //     public DynamicEntity1(IDatabaseProvider databaseProvider)
    //     {
    //         this.databaseProvider = databaseProvider;
    //     }

    //     public void SetValue(string propertyName, object value)
    //     {
    //         if (properties.ContainsKey(propertyName))
    //         {
    //             properties[propertyName] = value;
    //         }
    //     }

    //     public void Insert(string connectionString, string tableName)
    //     {
    //         databaseProvider.Insert(connectionString, tableName);
    //     }

    //     public void Update(string connectionString, string tableName, Dictionary<string, object> primaryKeyValues)
    //     {
    //         databaseProvider.Update(connectionString, tableName, primaryKeyValues);
    //     }

    //     public void Delete(string connectionString, string tableName, Dictionary<string, object> primaryKeyValues)
    //     {
    //         databaseProvider.Delete(connectionString, tableName, primaryKeyValues);
    //     }

    //     public List<Dictionary<string, object>> Select(string connectionString, string tableName)
    //     {
    //         return databaseProvider.Select(connectionString, tableName);
    //     }

    //     public List<TableInfo> GetTableList(string connectionString)
    //     {
    //         return databaseProvider.GetTableList(connectionString);
    //     }

    //     public TableInfo GetTableInfo(string connectionString, string tableName)
    //     {
    //         return databaseProvider.GetTableInfo(connectionString, tableName);
    //     }
    // }

    public class DynamicEntity1
    {
        private readonly IDatabaseProvider databaseProvider;

        private TableInfo _currentTableInfo;
        public TableInfo CurrentTableInfo { get => _currentTableInfo; set => _currentTableInfo = value; }

        public DynamicEntity1(IDatabaseProvider databaseProvider, string connectionString, string schemaName="")
        {
            databaseProvider.ConnectionString = connectionString;
            databaseProvider.SchemaName = schemaName;
            this.databaseProvider = databaseProvider;
        }

        public void Insert(string tableName, Dictionary<string, object> values)
        {
            databaseProvider.Insert(tableName, values);
        }

        public void Update(string tableName, Dictionary<string, object> primaryKeyValues, Dictionary<string, object> valuesToUpdate)
        {
            databaseProvider.Update(tableName, primaryKeyValues, valuesToUpdate);
        }

        public void Delete(string tableName, Dictionary<string, object> primaryKeyValues)
        {
            databaseProvider.Delete(tableName, primaryKeyValues);
        }

        public List<Dictionary<string, object>> Select(string tableName, string[] columns = null, List<Condition> whereConditions = null)
        {
            return databaseProvider.Select(tableName, columns, whereConditions);
        }

        public List<TableInfo> GetTableList()
        {
            return databaseProvider.GetTableList();
        }

        public Dictionary<string, TableInfo> GetDicTableInfo()
        {
            if (databaseProvider.DicTableInfo == null) {
                databaseProvider.GetTableList();
            }
            return databaseProvider.DicTableInfo;
        }

        public TableInfo GetTableInfo(string tableName)
        {
            _currentTableInfo = databaseProvider.DicTableInfo[tableName];
            return databaseProvider.GetTableInfo(tableName);
        }
    }
}