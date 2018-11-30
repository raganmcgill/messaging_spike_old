using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using common_models;
using database_registry.service.messages;
using helpers;
using message_types;
using MassTransit;
using Newtonsoft.Json;

namespace database_registry.service.consumers
{
    internal class RegisterDatabaseConsumer : IConsumer<RegisterDatabase>
    {

        private IBusControl bus;

        public RegisterDatabaseConsumer()
        {
            bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            bus.Start();
        }

        public Task Consume(ConsumeContext<RegisterDatabase> context)
        {
            ConsoleAppHelper.PrintHeader("Header.txt");

            var tableDefinition = GetSchemaInfo(context.Message);

            StoreDatabaseDefinition(context.Message, tableDefinition);

            PublishTables(context.Message, tableDefinition);

            Console.WriteLine($"Registered database");

            return Task.CompletedTask;
        }

        private void PublishTables(RegisterDatabase database, List<Table> tables)
        {
            var message = new DatabaseRegisteredMessage
            {
                Database = database,
                Schema = new Schema { Tables = tables }
            };

            bus.Publish<DatabaseRegistered>(message);
        }

        private void StoreDatabaseDefinition(RegisterDatabase database, List<Table> tables)
        {
            string subPath = $@"C:\dev\Stores\SchemaScanner\{database.Server}\{database.Database}"; // your code goes here

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
            string queryString = System.IO.File.ReadAllText("SchemaSQL.sql");
            string connectionString = $"Data Source={database.Server};Initial Catalog={database.Database};Integrated Security=False;User ID={database.Username};Password={database.Password};Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
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
