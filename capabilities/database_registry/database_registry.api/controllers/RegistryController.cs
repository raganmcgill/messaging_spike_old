using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using common_models;
using database_registry.api.models;
using message_types.commands;
using MassTransit;

namespace database_registry.api.controllers
{
    public class RegistryController : ApiController
    {
        private readonly IBusControl _bus;
        private readonly string _rabbitMqAddress = ConfigurationManager.AppSettings["RabbitHost"];
        private static readonly string RabbitUsername = ConfigurationManager.AppSettings["RabbitUserName"];
        private static readonly string RabbitPassword = ConfigurationManager.AppSettings["RabbitPassword"];

        public RegistryController()
        {
            var rabbitMqRootUri = new Uri(_rabbitMqAddress);

            _bus = Bus.Factory.CreateUsingRabbitMq(rabbit =>
                           {
                               rabbit.Host(rabbitMqRootUri, settings =>
                               {
                                   settings.Username(RabbitUsername);
                                   settings.Password(RabbitPassword);
                               });
                           });
        }

        [HttpPost]
        [Route("add")]
        public void Post(DatabaseConnectionDetails value)
        {
            Task<ISendEndpoint> sendEndpointTask = _bus.GetSendEndpoint(new Uri(string.Concat(_rabbitMqAddress, "/Registrations")));

            ISendEndpoint sendEndpoint = sendEndpointTask.Result;

            var dto = new DTO
            {
                ConnectionDetails = new ConnectionDetails
                {
                    Server = value.Server,
                    Database = value.Database,
                    User = value.User,
                    Password = value.Password
                }
            };
            Task sendTask = sendEndpoint.Send<RegisterDatabase>(dto);
        }
    }
}
