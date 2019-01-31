using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using common_models;
using helpers;
using message_types.commands;
using message_types.events;
using MassTransit;
using Newtonsoft.Json;

namespace dashboard.service.consumers
{
    internal class SchemaUpdateConsumer : IConsumer<SchemaChanged>
    {

        public Task Consume(ConsumeContext<SchemaChanged> context)
        {
//            ConsoleAppHelper.PrintHeader("Header.txt");

            var schema = context.Message.Schema;

            foreach (var table in schema.Tables)
            {
                Console.WriteLine(table.Name);
            }

            StoreDatabaseDefinition(context.Message.Database, schema.Tables);

            return Task.CompletedTask;
        }

        private void StoreDatabaseDefinition(RegisterDatabase database, List<Table> tables)
        {
            string subPath = $@"C:\dev\Stores\Dashboard\{database.Server}\{database.Database}";

            if (Directory.Exists(subPath))
            {
                Directory.Delete(subPath, true);
            }

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
    }
}