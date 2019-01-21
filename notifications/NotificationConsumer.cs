using System;
using System.Threading.Tasks;
using message_types.commands;
using MassTransit;
using Tulpep.NotificationWindow;

namespace notifications
{
    public class NotificationConsumer : IConsumer<NotifyUser>
    {

        private IBusControl bus;

        public NotificationConsumer()
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

        [MTAThread]
        public Task Consume(ConsumeContext<NotifyUser> context)
        {
            Form1.pop();

            return Task.CompletedTask;
        }

    }
}