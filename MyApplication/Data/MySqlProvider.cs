// MySqlProvider.cs

using MySqlConnector;
using System;
using System.Collections.Generic;

namespace MyApplication.Data
{
    public class MySqlProvider : IDatabaseProvider
    {
        private string _connectionString = "";

        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        private string _schemaName = "";

        public string SchemaName
        {
            get => _schemaName;
            set => _schemaName = value;
        }

        public void Insert(string tableName, Dictionary<string, object> valuesToInsert)
        {
            Console.WriteLine("Insert");
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string columns = string.Join(", ", valuesToInsert.Keys);
                string parameters = string.Join(", ", valuesToInsert.Keys.Select(k => $"@{k}"));

                string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";
                Console.WriteLine(sql);
                
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    foreach (var kvp in valuesToInsert)
                    {
                        command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(string tableName, Dictionary<string, object> whereKeyValues, Dictionary<string, object> valuesToUpdate)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string setClause = string.Join(", ", valuesToUpdate.Keys.Select(k => $"{k} = @{k}"));
                string whereClause = string.Join(" AND ", whereKeyValues.Keys.Select(k => $"{k} = @{k}"));

                string sql = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    foreach (var kvp in valuesToUpdate)
                    {
                        command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                    }

                    foreach (var kvp in whereKeyValues)
                    {
                        command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(string tableName, Dictionary<string, object> whereKeyValues)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string whereClause = string.Join(" AND ", whereKeyValues.Keys.Select(k => $"{k} = @{k}"));

                string sql = $"DELETE FROM {tableName} WHERE {whereClause}";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    foreach (var kvp in whereKeyValues)
                    {
                        command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Dictionary<string, object>> Select(string tableName, string[] columns = null, List<Condition> whereConditions = null)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string columnList = columns != null && columns.Length > 0 ? string.Join(", ", columns) : "*";
                string whereClause = whereConditions != null && whereConditions.Count > 0
                    ? "WHERE " + string.Join(" AND ", whereConditions.Select(c => $"{c.ColumnName} {GetOperator(c.Operator)} @{c.ColumnName}"))
                    : "";

                string sql = $"SELECT {columnList} FROM {tableName} {whereClause}";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    if (whereConditions != null)
                    {
                        foreach (var condition in whereConditions)
                        {
                            command.Parameters.AddWithValue($"@{condition.ColumnName}", condition.Value);
                        }
                    }

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }

                            result.Add(row);
                        }

                        return result;
                    }
                }
            }
        }

        private string GetOperator(ComparisonOperator comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.Equal:
                    return "=";
                case ComparisonOperator.NotEqual:
                    return "<>";
                case ComparisonOperator.LessThan:
                    return "<";
                case ComparisonOperator.LessThanOrEqual:
                    return "<=";
                case ComparisonOperator.GreaterThan:
                    return ">";
                case ComparisonOperator.GreaterThanOrEqual:
                    return ">=";
                default:
                    throw new ArgumentException($"Unsupported operator: {comparisonOperator}");
            }
        }

        public List<TableInfo> GetTableList()
        {
            List<TableInfo> tableList = new List<TableInfo>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SHOW TABLES";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader[0].ToString();
                            TableInfo tableInfo = GetTableInfo(tableName);
                            tableList.Add(tableInfo);
                        }
                    }
                }
            }

            return tableList;
        }

        public TableInfo GetTableInfo(string tableName)
        {
            TableInfo tableInfo = new TableInfo
            {
                TableName = tableName
            };

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = $"DESCRIBE {tableName}";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ColumnInfo columnInfo = new ColumnInfo
                            {
                                ColumnName = reader["Field"].ToString(),
                                DataType = reader["Type"].ToString(),
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
}
