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
        public static Database GetSchemaInfo(ConnectionDetails connectionDetails)
        {
            var database = new Database(){Name = connectionDetails.Database, ConnectionDetails = connectionDetails};


            var queryString = File.ReadAllText("SchemaSQL.sql");
            var connectionString = $"Data Source={connectionDetails.Server};Initial Catalog={connectionDetails.Database};Integrated Security=False;User ID={connectionDetails.User};Password={connectionDetails.Password};Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

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

                        if (!database.Tables.Any(x => x.Name == tableName))
                        {
                            var table = new Table
                            {
                                Schema = reader["TABLE_SCHEMA"].ToString(),
                                Name = tableName
                            };

                            database.Tables.Add(table);
                        }

                        var tableToEdit = database.Tables.FirstOrDefault(x => x.Name == tableName);

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

            return database;
        }
    }
}