// IDatabaseProvider.cs

using System.Collections.Generic;

namespace MyApplication.Data
{
    // public interface IDatabaseProvider
    // {
    //     // void AddProperty(string propertyName, object value);

    //     // object GetProperty(string propertyName);

    //     // void SetValue(string propertyName, object value);

    //     // object GetValue(string propertyName);

    //     void Insert(string tableName);
    //     void Update(string tableName, Dictionary<string, object> whereKeyValues);
    //     void Delete(string tableName, Dictionary<string, object> whereKeyValues);
    //     List<Dictionary<string, object>> Select(string tableName);

    //     List<TableInfo> GetTableList(string connectionString);

    //     TableInfo GetTableInfo(string tableName);
    // }

    public interface IDatabaseProvider
    {
        string ConnectionString {
            get;
            set;
        }
        string SchemaName {
            get;
            set;
        }
        void Insert(string tableName, Dictionary<string, object> valuesToInsert);
        void Update(string tableName, Dictionary<string, object> whereKeyValues, Dictionary<string, object> valuesToUpdate);
        void Delete(string tableName, Dictionary<string, object> whereKeyValues);
        List<Dictionary<string, object>> Select(string tableName, string[] columns = null, List<Condition> whereConditions = null);
        List<TableInfo> GetTableList();
        TableInfo GetTableInfo(string tableName);
    }

    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        // ... (other properties)
    }

    public class TableInfo
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
    }
}