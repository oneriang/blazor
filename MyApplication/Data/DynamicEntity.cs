// DynamicEntity.cs

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace BlazorApp.Data
{
    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        // ... (other properties)
    }

    class TableInfo
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();

        public static TableInfo GetTableInfo(string connectionString, string tableName)
        {
            TableInfo tableInfo = new TableInfo
            {
                TableName = tableName
            };

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = $"PRAGMA table_info({tableName})";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ColumnInfo columnInfo = new ColumnInfo
                            {
                                ColumnName = reader["name"].ToString(),
                                DataType = reader["type"].ToString(),
                                // ... (other properties)
                            };

                            tableInfo.Columns.Add(columnInfo);
                        }
                    }
                }
            }

            return tableInfo;
        }
    }

    class DynamicEntity
    {
        public Dictionary<string, object> properties = new Dictionary<string, object>();

        public void AddProperty(string propertyName, object value)
        {
            properties.Add(propertyName, value);
        }

        public object GetProperty(string propertyName)
        {
            return properties.ContainsKey(propertyName) ? properties[propertyName] : null;
        }

        public void SetValue(string propertyName, object value)
        {
            if (properties.ContainsKey(propertyName))
            {
                properties[propertyName] = value;
            }
        }

        public object GetValue(string propertyName)
        {
            return properties.ContainsKey(propertyName) ? properties[propertyName] : null;
        }

        public void Insert(string connectionString, string tableName)
        {
            Console.WriteLine("Insert");
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string columns = string.Join(", ", properties.Keys);
                string values = string.Join(", ", properties.Keys.Select(key => "@" + key));
                string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

                using (SqliteCommand command = new SqliteCommand(insertQuery, connection))
                {
                    foreach (var entry in properties)
                    {
                        command.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                    }

                    command.ExecuteNonQuery();
                    Console.WriteLine("command.ExecuteNonQuery()");
                }
            }
        }

        public void Update(string connectionString, string tableName, Dictionary<string, object> primaryKeyValues)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string setClause = string.Join(", ", properties.Keys.Select(key => $"{key} = @{key}"));
                string whereClause = string.Join(" AND ", primaryKeyValues.Keys.Select(key => $"{key} = @{key}"));

                string updateQuery = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

                using (SqliteCommand command = new SqliteCommand(updateQuery, connection))
                {
                    foreach (var entry in properties)
                    {
                        command.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                    }

                    foreach (var entry in primaryKeyValues)
                    {
                        command.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(string connectionString, string tableName, Dictionary<string, object> primaryKeyValues)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string whereClause = string.Join(" AND ", primaryKeyValues.Keys.Select(key => $"{key} = @{key}"));
                string deleteQuery = $"DELETE FROM {tableName} WHERE {whereClause}";

                using (SqliteCommand command = new SqliteCommand(deleteQuery, connection))
                {
                    foreach (var entry in primaryKeyValues)
                    {
                        command.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Dictionary<string, object>> Select(string connectionString, string tableName)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = $"SELECT * FROM {tableName}";

                using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object value = reader.GetValue(i);
                                row.Add(columnName, value);
                            }

                            results.Add(row);
                        }
                    }
                }
            }

            return results;
        }

        public static List<TableInfo> GetTableList(string connectionString)
        {
            List<TableInfo> tableList = new List<TableInfo>();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT name FROM sqlite_master WHERE type='table'";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader["name"].ToString();
                            TableInfo tableInfo = TableInfo.GetTableInfo(connectionString, tableName);
                            tableList.Add(tableInfo);
                        }
                    }
                }
            }

            return tableList;
        }
    }
}
