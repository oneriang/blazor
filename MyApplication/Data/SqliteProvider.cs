// SqliteProvider.cs

using Microsoft.Data.Sqlite;

namespace MyApplication.Data
{
    public class SqliteProvider : IDatabaseProvider
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
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string columns = string.Join(", ", valuesToInsert.Keys);
                string parameters = string.Join(", ", valuesToInsert.Keys.Select(k => $"@{k}"));

                string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";
                Console.WriteLine(sql);

                using (SqliteCommand command = new SqliteCommand(sql, connection))
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
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string setClause = string.Join(", ", valuesToUpdate.Keys.Select(k => $"{k} = @{k}"));
                string whereClause = string.Join(" AND ", whereKeyValues.Keys.Select(k => $"{k} = @{k}"));

                string sql = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

                using (SqliteCommand command = new SqliteCommand(sql, connection))
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
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string whereClause = string.Join(" AND ", whereKeyValues.Keys.Select(k => $"{k} = @{k}"));

                string sql = $"DELETE FROM {tableName} WHERE {whereClause}";

                using (SqliteCommand command = new SqliteCommand(sql, connection))
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
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string columnList = columns != null && columns.Any() ? string.Join(", ", columns) : "*";
                string whereClause = whereConditions != null && whereConditions.Any()
                    ? "WHERE " + string.Join(" AND ", whereConditions.Select(c => $"{c.ColumnName} {GetOperator(c.Operator)} @{c.ColumnName}"))
                    : "";

                string sql = $"SELECT {columnList} FROM {tableName} {whereClause}";

                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    if (whereConditions != null)
                    {
                        foreach (var condition in whereConditions)
                        {
                            command.Parameters.AddWithValue($"@{condition.ColumnName}", condition.Value);
                        }
                    }

                    using (SqliteDataReader reader = command.ExecuteReader())
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

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
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

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
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

}