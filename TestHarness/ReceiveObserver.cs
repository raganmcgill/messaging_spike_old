﻿using System;
using System.Threading.Tasks;
using MassTransit;

namespace TestHarness
{
    partial class Program
    {
        public class ReceiveObserver : IReceiveObserver
        {
            private readonly string _logger = "[MessageReceiveObserver]";

            public async Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
            {
                Console.WriteLine(string.Concat(_logger, ": A fault is consumed for type ", context.Message.GetType(), ". Consumer type: ", consumerType, ", exception message: ", exception.Message));
                await context.CompleteTask;
            }

            public async Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
            {
                Console.WriteLine(string.Concat(_logger, ": A message is consumed for type ", context.Message.GetType(),
                    ". Consumer type: ", consumerType, ", duration in millis: ", duration.TotalMilliseconds));
                await context.CompleteTask;
            }

            public async Task PostReceive(ReceiveContext context)
            {
                Console.WriteLine(string.Concat(_logger, ": A message is past the receive stage. Has been delivered to at least one consumer: ",
                    context.IsDelivered));
                await context.CompleteTask;
            }

            public async Task PreReceive(ReceiveContext context)
            {
                Console.WriteLine(string.Concat(_logger, ": A message is before the receive stage. Has been delivered to at least one consumer: ",
                    context.IsDelivered));
                await context.CompleteTask;
            }

            public async Task ReceiveFault(ReceiveContext context, Exception exception)
            {
                Console.WriteLine(string.Concat(_logger, ": A fault is received with exception ", exception.Message));
                await context.CompleteTask;
            }
        }
    }
    
}
