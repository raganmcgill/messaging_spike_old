using System;
using database_registry.service.consumers;
using helpers;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace database_registry.service
{
    class Program
    {
        static void Main(string[] args)
        {
            RunMassTransitReceiverWithRabbit();
        }

        private static void RunMassTransitReceiverWithRabbit()
        {
            ConsoleAppHelper.PrintHeader("Header.txt");

            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                IRabbitMqHost rabbitMqHost = rabbit.Host(new Uri("rabbitmq://localhost"), settings =>
                {
                    settings.Password("guest");
                    settings.Username("guest");
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, "redgate.queues", conf =>
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
