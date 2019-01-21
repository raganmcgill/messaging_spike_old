using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using database_registry.api.models;
using message_types;
using message_types.commands;
using MassTransit;
using RabbitMQ.Client;

namespace database_registry.api.controllers
{
    public class RegistryController : ApiController
    {
        private IBusControl bus;
        private string rabbitMqAddress = "rabbitmq://localhost";
        private string rabbitMqQueue = "redgate.queues";

        public RegistryController()
        {

            Uri rabbitMqRootUri = new Uri(rabbitMqAddress);

            bus = Bus.Factory.CreateUsingRabbitMq(rabbit =>
                           {
                               rabbit.Host(rabbitMqRootUri, settings =>
                               {
                                   settings.Password("guest");
                                   settings.Username("guest");
                               });
                           });


            //            bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            //            {
            //                var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
            //                {
            //                    h.Username("guest");
            //                    h.Password("guest");
            //                });
            //
            //                //                cfg.Send<RegisterDatabase>(x => { x.UseRoutingKeyFormatter(context => "routingKey"); });
            //                //                cfg.Message<RegisterDatabase>(x => x.SetEntityName("TestMessage"));
            //                //                cfg.Publish<RegisterDatabase>(x => { x.ExchangeType = ExchangeType.Direct; });
            //                //                cfg.ReceiveEndpoint(host, "TestMessage_Queue", e =>
            //                //                {
            //                //                    e.BindMessageExchanges = false;
            //                //                    e.Bind("TestMessage", x =>
            //                //                    {
            //                //                        x.ExchangeType = ExchangeType.Direct;
            //                //                        x.RoutingKey = "routingKey";
            //                //                    });
            //                //                });
            //
            //            });
            //
            //
            //            bus.Start();
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
            //var db = new DatabaseConnectionDetails { Server = value.Server, Database = value.Database, User = value.User, Password = value.Password };

            //            bus.Publish<RegisterDatabase>(db);

            Task<ISendEndpoint> sendEndpointTask = bus.GetSendEndpoint(new Uri(string.Concat(rabbitMqAddress, "/", rabbitMqQueue)));

            ISendEndpoint sendEndpoint = sendEndpointTask.Result;

            Task sendTask = sendEndpoint.Send<RegisterDatabase>(new {Server = value.Server, Database = value.Database, User = value.User, Password = value.Password });
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
