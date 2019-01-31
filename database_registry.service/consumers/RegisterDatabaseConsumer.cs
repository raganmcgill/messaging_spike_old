using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using common_models;
using helpers;
using message_types.commands;
using message_types.events;
using monitor.service.messages;
using monitor.service.models;
using MassTransit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace monitor.service.consumers
{
    internal class RegisterDatabaseConsumer : IConsumer<RegisterDatabase>
    {
        public Task Consume(ConsumeContext<RegisterDatabase> context)
        {
            ConsoleAppHelper.PrintHeader("Header.txt");

            var schema = Helper.Moo(context.Message);

            PublishTables(context.Message, schema, context);

            Console.WriteLine($"Registered database with {schema.Tables.Count} tables at {DateTime.Now}");

            return Task.CompletedTask;
        }

        private void PublishTables(RegisterDatabase database, Schema schema, ConsumeContext<RegisterDatabase> context)
        {
            var message = new DatabaseRegisteredMessage
            {
                Database = database,
                Schema = schema
            };

            context.Publish<DatabaseRegistered>(message);
        }

    }
}
