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
        private static readonly string RabbitMqGeneralQueueName = "redgate.queues";
        //private static readonly string RabbitMqNotificationQueueName = "redgate.notifications";
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

                rabbit.ReceiveEndpoint(rabbitMqHost, RabbitMqGeneralQueueName, conf => { conf.Consumer<DatabaseRegisteredConsumer>(); });
//                rabbit.ReceiveEndpoint(rabbitMqHost, RabbitMqNotificationQueueName, conf => { conf.Consumer<NotificationConsumer>(); });

            });

            rabbitBusControl.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            rabbitBusControl.Stop();
        }
    }
}