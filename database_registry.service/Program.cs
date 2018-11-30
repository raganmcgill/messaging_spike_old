using System;
using database_registry.service.consumers;
using helpers;
using MassTransit;

namespace database_registry.service
{
    class Program
    {
        static void Main(string[] args)
        {
            string exchangeName = "DatabaseRegistry";
            string queueName = "RegisterDatabase";

            ConsoleAppHelper.PrintHeader("Header.txt");

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, queueName, ep =>
                {
                    //ep.Bind(exchangeName);
                    ep.Consumer(() => new RegisterDatabaseConsumer());
                });

            });

            bus.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            bus.Stop();
        }
    }
}
