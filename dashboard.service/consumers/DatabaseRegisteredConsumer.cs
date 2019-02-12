using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using common_models;
using helpers;
using message_types;
using message_types.commands;
using message_types.events;
using MassTransit;
using Newtonsoft.Json;

namespace dashboard.service.consumers
{
    internal class DatabaseRegisteredConsumer : IConsumer<DatabaseRegistered>
    {

        public Task Consume(ConsumeContext<DatabaseRegistered> context)
        {
            ConsoleAppHelper.PrintHeader("Header.txt");

            var schema = context.Message.Schema;

            foreach (var table in schema.Tables)
            {
                Console.WriteLine(table.Name);
            }

            StorageHelper.StoreDatabaseDefinition(context.Message.Database, schema.Tables,"Dashboard");

            return Task.CompletedTask;
        }

       
    }
}
