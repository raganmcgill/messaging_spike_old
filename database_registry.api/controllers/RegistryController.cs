using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using database_registry.api.models;
using message_types.commands;
using MassTransit;

namespace database_registry.api.controllers
{
    public class RegistryController : ApiController
    {
        private readonly IBusControl _bus;
        private readonly string _rabbitMqAddress = ConfigurationManager.AppSettings["RabbitHost"];
        private readonly string _rabbitMqQueue = ConfigurationManager.AppSettings["RabbitQueue"];
        private static readonly string RabbitUsername = ConfigurationManager.AppSettings["RabbitUserName"];
        private static readonly string RabbitPassword = ConfigurationManager.AppSettings["RabbitPassword"];

        public RegistryController()
        {
            var rabbitMqRootUri = new Uri(_rabbitMqAddress);

            _bus = Bus.Factory.CreateUsingRabbitMq(rabbit =>
                           {
                               rabbit.Host(rabbitMqRootUri, settings =>
                               {
                                   settings.Password(RabbitUsername);
                                   settings.Username(RabbitPassword);
                               });
                           });
        }

        [HttpPost]
        [Route("add")]
        public void Post(DatabaseConnectionDetails value)
        {
            Task<ISendEndpoint> sendEndpointTask = _bus.GetSendEndpoint(new Uri(string.Concat(_rabbitMqAddress, "/", _rabbitMqQueue)));

            ISendEndpoint sendEndpoint = sendEndpointTask.Result;

            Task sendTask = sendEndpoint.Send<RegisterDatabase>(new { Server = value.Server, Database = value.Database, User = value.User, Password = value.Password });
        }

        [HttpPost]
        [Route("delete")]
        public void Delete([FromBody] string databaseName)
        {
            var db = new DatabaseConnectionDetails { Database = databaseName };

            _bus.Publish<DeleteDatabase>(db);
        }
    }
}
