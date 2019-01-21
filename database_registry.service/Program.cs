using System;
using System.Configuration;
using database_registry.service.consumers;
using helpers;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace database_registry.service
{
    class Program
    {
        private static readonly string RabbitMqAddress = "rabbitmq://localhost";
        private static readonly string RabbitMqQueue = "redgate.queues";
        private static readonly string RabbitUsername = ConfigurationManager.AppSettings["RabbitUserName"];
        private static readonly string RabbitPassword = ConfigurationManager.AppSettings["RabbitPassword"];

        static void Main(string[] args)
        {
            ConsoleAppHelper.PrintHeader("Header.txt");

            RunMassTransitReceiverWithRabbit();
        }

        private static void RunMassTransitReceiverWithRabbit()
        {

            var rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                var rabbitMqHost = rabbit.Host(new Uri(RabbitMqAddress), settings =>
                {
                    settings.Password(RabbitUsername);
                    settings.Username(RabbitPassword);
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, RabbitMqQueue, conf =>
                {
                    conf.Consumer<RegisterDatabaseConsumer>();
                });
            });

            rabbitBusControl.Start();


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            rabbitBusControl.Stop();
        }
    }
}
