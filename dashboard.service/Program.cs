using System;
using System.IO;
using System.Reflection;
using dashboard.service.consumers;
using helpers;
using MassTransit;

namespace dashboard.service
{
    class Program
    {
        static void Main(string[] args)
        {
            //string exchangeName = "_DatabaseRegistry";
            Console.SetWindowSize(80, 20);
            ConsoleAppHelper.PrintHeader("Header.txt");

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "DatabaseRegistered", ep =>
                {
              //      ep.Bind(exchangeName);
                    ep.Consumer(() => new DatabaseRegisteredConsumer());
                });

                sbc.ReceiveEndpoint(host, "SchemaChanged", ep =>
                {
                //    ep.Bind(exchangeName);
                    ep.Consumer<SchemaUpdateConsumer>();
                });

            });

            bus.Start();
        }
    }
}
