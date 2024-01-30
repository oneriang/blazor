// DynamicEntity.cs

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace BlazorApp.Data
{
    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        // ... (其他属性)
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

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = $"PRAGMA table_info({tableName})";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ColumnInfo columnInfo = new ColumnInfo
                            {
                                ColumnName = reader["name"].ToString(),
                                DataType = reader["type"].ToString(),
                                // ... (其他属性)
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
        private Dictionary<string, object> properties = new Dictionary<string, object>();

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
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string columns = string.Join(", ", properties.Keys);
                string values = string.Join(", ", properties.Keys.Select(key => "@" + key));
                string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    foreach (var entry in properties)
                    {
                        command.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        // public void Update(string connectionString, string tableName, string primaryKeyColumnName, object primaryKeyValue)
        // {
        //     using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //     {
        //         connection.Open();

        //         string setClause = string.Join(", ", properties.Keys.Select(key => $"{key} = @{key}"));
        //         string updateQuery = $"UPDATE {tableName} SET {setClause} WHERE {primaryKeyColumnName} = @{primaryKeyColumnName}";

        //         using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
        //         {
        //             foreach (var entry in properties)
        //             {
        //                 command.Parameters.AddWithValue("@" + entry.Key, entry.Value);
        //             }

        //             command.Parameters.AddWithValue("@" + primaryKeyColumnName, primaryKeyValue);

        //             command.ExecuteNonQuery();
        //         }
        //     }
        // }

        // public void Delete(string connectionString, string tableName, string primaryKeyColumnName, object primaryKeyValue)
        // {
        //     using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        //     {
        //         connection.Open();

        //         string deleteQuery = $"DELETE FROM {tableName} WHERE {primaryKeyColumnName} = @{primaryKeyColumnName}";

        //         using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
        //         {
        //             command.Parameters.AddWithValue("@" + primaryKeyColumnName, primaryKeyValue);

        //             command.ExecuteNonQuery();
        //         }
        //     }
        // }

        public void Update(string connectionString, string tableName, Dictionary<string, object> primaryKeyValues)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string setClause = string.Join(", ", properties.Keys.Select(key => $"{key} = @{key}"));
                string whereClause = string.Join(" AND ", primaryKeyValues.Keys.Select(key => $"{key} = @{key}"));

                string updateQuery = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
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
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string whereClause = string.Join(" AND ", primaryKeyValues.Keys.Select(key => $"{key} = @{key}"));
                string deleteQuery = $"DELETE FROM {tableName} WHERE {whereClause}";

                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
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

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectQuery = $"SELECT * FROM {tableName}";
                Console.WriteLine("tableName");
                Console.WriteLine(tableName);
Console.WriteLine(selectQuery);
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
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
            // List<string> tableList = new List<string>();
            List<TableInfo> tableList = new List<TableInfo>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT name FROM sqlite_master WHERE type='table'";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // // TableInfo tableInfo = new TableInfo
                            // // {
                            // //     TableName = reader["name"].ToString(),
                            // //     // ... (其他属性)
                            // // };

                            // // tableList.Add(tableInfo);
                            // tableList.Add(reader["name"].ToString());

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
