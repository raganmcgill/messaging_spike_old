
using System;
using MassTransit;
using System.Configuration;

namespace suggestions
{
    class Program
    {
        private static readonly string RabbitMqAddress = "rabbitmq://localhost";
        private static readonly string RabbitMqSuggestionsQueueName = "suggestions";
        private static readonly string RabbitUsername = ConfigurationManager.AppSettings["RabbitUserName"];
        private static readonly string RabbitPassword = ConfigurationManager.AppSettings["RabbitPassword"];
        static void Main(string[] args)
        {
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

                rabbit.ReceiveEndpoint(rabbitMqHost, RabbitMqSuggestionsQueueName, conf => { conf.Consumer<SuggestionsConsumer>(); });

            });

            rabbitBusControl.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            rabbitBusControl.Stop();
        }
    }
}
