using System;
using helpers;
using MassTransit;
using protector.consumers;

namespace protector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(80, 20);
            ConsoleAppHelper.PrintHeader("Header.txt");

            var _rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

//                sbc.ReceiveEndpoint(host, "ProtectorQueue", ep =>
//                {
//                    ep.Bind("DatabaseRegistered");
//                    ep.Consumer(() => new DatabaseRegisteredConsumer());
//                });
//
//                sbc.ReceiveEndpoint(host, "ProtectorQueue", ep =>
//                {
//                    ep.Bind("DatabaseRegistered");
//                    ep.Consumer(() => new DatabaseRegisteredConsumer());
//                });

                sbc.ReceiveEndpoint(host, "DatabaseRegistered", ep =>
                {
                    ep.Consumer(() => new DatabaseRegisteredConsumer());
                });

            });
            _rabbitBusControl.Start();


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            

            _rabbitBusControl.Stop();
        }
    }
}
