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
            var database = context.Message.Database;

            foreach (var table in database.Tables)
            {
                Console.WriteLine(table.Name);
            }

            StorageHelper.StoreDatabaseDefinition(context.Message.Database.ConnectionDetails, database.Tables, "Dashboard");

            return Task.CompletedTask;
        }
    }
}