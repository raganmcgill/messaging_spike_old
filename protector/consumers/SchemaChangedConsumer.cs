using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using common_models;
using helpers;
using message_types.events;
using MassTransit;

namespace protector.consumers
{
    internal class SchemaChangedConsumer : IConsumer<SchemaChanged>
    {
        public Task Consume(ConsumeContext<SchemaChanged> context)
        {
            ConsoleAppHelper.PrintHeader("Header.txt");
            Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss"));

            var sensitiveColumnNames = new List<string>
            {
                "firstname",
                "name",
                "address",
                "email"
            };

            var atRiskColumns = new List<Column>();

            var schema = context.Message.Database;

            foreach (var table in schema.Tables)
            {
                foreach (var tableColumn in table.Columns)
                {
                    if (sensitiveColumnNames.Contains(tableColumn.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        atRiskColumns.Add(tableColumn);
                        Console.WriteLine($"Sensitive column name found : {table.Name} - {tableColumn.Name}");
                    }
                }
            }

            return Task.CompletedTask;
        }
        

    }
}
