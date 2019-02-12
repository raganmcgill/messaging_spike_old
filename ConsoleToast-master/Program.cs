using System;
using System.Configuration;
using helpers;
using MassTransit;
using Notifications.consumers;

namespace Notifications
{
    class Program
    {
        private static readonly string RabbitMqAddress = "rabbitmq://localhost";
        //private static readonly string RabbitMqNotificationQueueName = "redgate.notifications";
        private static readonly string RabbitUsername = ConfigurationManager.AppSettings["RabbitUserName"];
        private static readonly string RabbitPassword = ConfigurationManager.AppSettings["RabbitPassword"];

        static void Main(string[] args)
        {
            Console.SetWindowSize(80, 20);
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

                rabbit.ReceiveEndpoint(rabbitMqHost, "RegistrationNotifications", conf =>
                {
                    conf.Bind("Registrations");
                    conf.Consumer<DatabaseRegisteredConsumer>();
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, "SchemaChangedNotifications", conf =>
                {
                    conf.Bind("SchemaChanged");
                    conf.Consumer<DatabaseUpdatedConsumer>();
                });

            });

            rabbitBusControl.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            rabbitBusControl.Stop();
        }
    }
}