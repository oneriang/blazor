// PostgreSqlProvider.cs

using Npgsql;

namespace MyApplication.Data
{
    public class PostgreSqlProvider : IDatabaseProvider
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
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string columns = string.Join(", ", valuesToInsert.Keys.Select(k => $"\"{k}\""));
                string parameters = string.Join(", ", valuesToInsert.Keys.Select(k => $"@{k}"));

                string sql = $"INSERT INTO {_schemaName}.\"{tableName}\" ({columns}) VALUES ({parameters})";
                Console.WriteLine(sql);

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
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
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string setClause = string.Join(", ", valuesToUpdate.Keys.Select(k => $"\"{k}\" = @{k}"));
                string whereClause = string.Join(" AND ", whereKeyValues.Keys.Select(k => $"\"{k}\" = @{k}"));

                string sql = $"UPDATE {_schemaName}.\"{tableName}\" SET {setClause} WHERE {whereClause}";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
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
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string whereClause = string.Join(" AND ", whereKeyValues.Keys.Select(k => $"\"{k}\" = @{k}"));

                string sql = $"DELETE FROM {_schemaName}.\"{tableName}\" WHERE {whereClause}";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
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
            Console.WriteLine("Select");
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string columnList = columns != null && columns.Length > 0 ? string.Join(", ", columns.Select(k => $"\"{k}\"")) : "*";
                string whereClause = whereConditions != null && whereConditions.Count > 0
                    ? "WHERE " + string.Join(" AND ", whereConditions.Select(c => $"\"{c.ColumnName}\" {GetOperator(c.Operator)} @{c.ColumnName}"))
                    : "";

                string sql = $"SELECT {columnList} FROM {_schemaName}.\"{tableName}\" {whereClause}";
                Console.WriteLine(sql);

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    if (whereConditions != null)
                    {
                        foreach (var condition in whereConditions)
                        {
                            command.Parameters.AddWithValue($"@{condition.ColumnName}", condition.Value);
                        }
                    }

                    using (NpgsqlDataReader reader = command.ExecuteReader())
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

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = $"SELECT table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema = '{_schemaName}'";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader["table_name"].ToString();
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

            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{_schemaName}.\"{tableName}\"'";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ColumnInfo columnInfo = new ColumnInfo
                            {
                                ColumnName = reader["COLUMN_NAME"].ToString(),
                                DataType = reader["DATA_TYPE"].ToString(),
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
