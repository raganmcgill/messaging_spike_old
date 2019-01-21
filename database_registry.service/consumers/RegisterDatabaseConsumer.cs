﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using common_models;
using database_registry.service.messages;
using helpers;
using message_types;
using message_types.commands;
using message_types.events;
using MassTransit;
using Newtonsoft.Json;

namespace database_registry.service.consumers
{
    internal class RegisterDatabaseConsumer : IConsumer<RegisterDatabase>
    {
        public Task Consume(ConsumeContext<RegisterDatabase> context)
        {
            ConsoleAppHelper.PrintHeader("Header.txt");

            var tableDefinition = GetSchemaInfo(context.Message);

            StoreDatabaseDefinition(context.Message, tableDefinition);

            PublishTables(context.Message, tableDefinition, context);

            Console.WriteLine($"Registered database with {tableDefinition.Count} tables at {DateTime.Now}");

            return Task.CompletedTask;
        }

        private void PublishTables(RegisterDatabase database, List<Table> tables, ConsumeContext<RegisterDatabase> context)
        {
            var message = new DatabaseRegisteredMessage
            {
                Database = database,
                Schema = new Schema { Tables = tables }
            };

            context.Publish<DatabaseRegistered>(message);
            
        }

        private void StoreDatabaseDefinition(RegisterDatabase database, List<Table> tables)
        {
            var subPath = $@"C:\dev\Stores\SchemaScanner\{database.Server}\{database.Database}";

            if (!Directory.Exists(subPath))
            {
                Directory.CreateDirectory(subPath);
            }

            foreach (var table in tables)
            {
                var filename = $"{table.Name}.txt";

                var serilisedTable = JsonConvert.SerializeObject(table);

                var path = Path.Combine(subPath, filename);

                File.WriteAllText(path, serilisedTable);
            }

        }

        public List<Table> GetSchemaInfo(RegisterDatabase database)
        {
            var tables = new List<Table>();
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

                        if (!tables.Any(x => x.Name == tableName))
                        {
                            var table = new Table
                            {
                                Schema = reader["TABLE_SCHEMA"].ToString(),
                                Name = tableName
                            };

                            tables.Add(table);
                        }

                        var tableToEdit = tables.FirstOrDefault(x => x.Name == tableName);

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

            return tables;
        }
    }
}
