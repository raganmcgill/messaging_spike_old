using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using common_models;
using message_types.commands;
using Newtonsoft.Json;

namespace monitor.service.models
{
    internal static class Helper
    {
        public static Schema GetSchemaInfo(RegisterDatabase database)
        {
            var schema = new Schema();
            var queryString = File.ReadAllText("SchemaSQL.sql");
            var connectionString = $"Data Source={database.Server};Initial Catalog={database.Database};Integrated Security=False;User ID={database.User};Password={database.Password};Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(queryString, connection);

                connection.Open();

                var reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        var tableName = reader["TABLE_NAME"].ToString();

                        if (!schema.Tables.Any(x => x.Name == tableName))
                        {
                            var table = new Table
                            {
                                Schema = reader["TABLE_SCHEMA"].ToString(),
                                Name = tableName
                            };

                            schema.Tables.Add(table);
                        }

                        var tableToEdit = schema.Tables.FirstOrDefault(x => x.Name == tableName);

                        if (tableToEdit != null)
                        {
                            var dataType = new DataType
                            {
                                Type = reader["DATA_TYPE"].ToString()
                            };

                            if (String.IsNullOrEmpty(reader["CHARACTER_MAXIMUM_LENGTH"].ToString()))
                            {
                                dataType.MaximumLength = null;
                            }
                            else
                            {
                                dataType.MaximumLength = Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]);
                            }

                            if (String.IsNullOrEmpty(reader["NUMERIC_PRECISION"].ToString()))
                            {
                                dataType.Precision = null;
                            }
                            else
                            {
                                dataType.Precision = Convert.ToInt32(reader["NUMERIC_PRECISION"]);
                            }

                            if (String.IsNullOrEmpty(reader["NUMERIC_PRECISION_RADIX"].ToString()))
                            {
                                dataType.PrecisionRadix = null;
                            }
                            else
                            {
                                dataType.PrecisionRadix = Convert.ToInt32(reader["NUMERIC_PRECISION_RADIX"]);
                            }

                            if (String.IsNullOrEmpty(reader["NUMERIC_SCALE"].ToString()))
                            {
                                dataType.NumericScale = null;
                            }
                            else
                            {
                                dataType.NumericScale = Convert.ToInt32(reader["NUMERIC_SCALE"]);
                            }

                            if (String.IsNullOrEmpty(reader["DATETIME_PRECISION"].ToString()))
                            {
                                dataType.DateTimePrecision = null;
                            }
                            else
                            {
                                dataType.DateTimePrecision = Convert.ToInt32(reader["DATETIME_PRECISION"]);
                            }


                            var column = new Column
                            {
                                Name = reader["COLUMN_NAME"].ToString(),
                                OrdinalPosition = Convert.ToInt32(reader["ORDINAL_POSITION"]),
                                Default = reader["COLUMN_DEFAULT"].ToString(),
                                DataType = dataType
                            };

                            tableToEdit.Columns.Add(column);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException());
                }
                finally
                {
                    reader.Close();
                }
            }

            return schema;
        }

        public static void StoreDatabaseDefinition(RegisterDatabase database, Schema schema)
        {
            var subPath = $@"C:\dev\Stores\monitor\{database.Server}\{database.Database}";

            if (Directory.Exists(subPath))
            {
                Directory.Delete(subPath, true);
            }

            if (!Directory.Exists(subPath))
            {
                Directory.CreateDirectory(subPath);
            }

            foreach (var table in schema.Tables)
            {
                var filename = $"{table.Name}.txt";

                var serilisedTable = JsonConvert.SerializeObject(table);

                var path = Path.Combine(subPath, filename);

                File.WriteAllText(path, serilisedTable);
            }

        }
    }
}