using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using dashboard.service.consumers;
using helpers;
using MassTransit;

namespace dashboard.service
{
    class Program
    {
        private static readonly string RabbitMqAddress = ConfigurationManager.AppSettings["RabbitHost"];
        private static readonly string RabbitUsername = ConfigurationManager.AppSettings["RabbitUserName"];
        private static readonly string RabbitPassword = ConfigurationManager.AppSettings["RabbitPassword"];

        static void Main(string[] args)
        {

            //string exchangeName = "_DatabaseRegistry";
            Console.SetWindowSize(80, 20);
            ConsoleAppHelper.PrintHeader("Header.txt");

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri(RabbitMqAddress), h =>
                {
                    h.Username(RabbitUsername);
                    h.Password(RabbitPassword);
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
