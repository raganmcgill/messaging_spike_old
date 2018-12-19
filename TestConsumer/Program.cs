using System;
using System.Threading.Tasks;
using helpers;
using message_types;
using message_types.events;
using MassTransit;

namespace TestConsumer
{

    internal class Consumer : IConsumer<DatabaseRegistered>
    {

        private IBusControl bus;

        public Consumer()
        {
            bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            bus.Start();
        }

        public Task Consume(ConsumeContext<DatabaseRegistered> context)
        {
            ConsoleAppHelper.PrintHeader("Header.txt");

            var schema = context.Message.Schema;

            foreach (var table in schema.Tables)
            {
                Console.WriteLine(table.Name);
            }
            

            return Task.CompletedTask;
        }
        
    }

    class Program
    {
        static void Main(string[] args)
        {

            string exchangeName = "DatabaseRegistry";
            string queueName = "Sandbox";

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
                    ep.Bind(exchangeName);
                    ep.Consumer(() => new Consumer());
                });

            });

            bus.Start();
        }
    }
}
