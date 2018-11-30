using System;
using System.Collections.Generic;
using System.Web.Http;
using database_registry.api.models;
using message_types;
using MassTransit;

namespace database_registry.api.controllers
{
    public class RegistryController : ApiController
    {
        private IBusControl bus;

        public RegistryController()
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

        [HttpGet]
        [Route("list")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [HttpPost]
        [Route("add")]
        public void Post(DatabaseConnectionDetails value)
        {
            var db = new DatabaseConnectionDetails { Server = value.Server, Database = value.Database, User = value.User };

            bus.Publish<RegisterDatabase>(db);
        }

        [HttpPost]
        [Route("delete")]
        public void Delete([FromBody] string databaseName)
        {
            var db = new DatabaseConnectionDetails { Database = databaseName };

            bus.Publish<DeleteDatabase>(db);
        }
    }
}
