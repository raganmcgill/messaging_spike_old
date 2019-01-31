using System;
using System.IO;
using System.Threading.Tasks;
using helpers;
using message_types.commands;
using MassTransit;
using Newtonsoft.Json;

namespace hospital.consumers
{
    internal class HeartbeatConsumer : IConsumer<Heartbeat>
    {

        public Task Consume(ConsumeContext<Heartbeat> context)
        {
            //ConsoleAppHelper.PrintHeader("Header.txt");

            var heartBeat = context.Message;

            Store(heartBeat);

            //Console.WriteLine(DateTime.Now);

            return Task.CompletedTask;
        }

        private void Store(Heartbeat heartBeat)
        {
            var subPath = $@"C:\dev\Stores\hospital\";

            if (!Directory.Exists(subPath))
            {
                Directory.CreateDirectory(subPath);
            }

            var filename = $"{heartBeat.Name}.txt";

            var serilisedHeartbeat = JsonConvert.SerializeObject(heartBeat);

            var path = Path.Combine(subPath, filename);

            File.WriteAllText(path, serilisedHeartbeat);
        }
    }
}