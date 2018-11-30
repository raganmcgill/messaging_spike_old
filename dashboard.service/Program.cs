﻿using System;
using dashboard.service.consumers;
using helpers;
using MassTransit;

namespace dashboard.service
{
    class Program
    {
        static void Main(string[] args)
        {
            //string exchangeName = "DatabaseRegistry";
            string queueName = "DatabaseRegistered";

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
                    ep.Consumer(() => new DatabaseRegisteredConsumer());
                });

            });

            bus.Start();
        }
    }
}
