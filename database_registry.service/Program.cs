using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Timers;
using common_models;
using helpers;
using monitor.service.consumers;
using monitor.service.models;
using MassTransit;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace monitor.service
{
    class Program
    {
        private static readonly string RabbitMqAddress = ConfigurationManager.AppSettings["RabbitHost"];
        private static readonly string RabbitMqQueue = ConfigurationManager.AppSettings["RabbitQueue"];
        private static readonly string RabbitUsername = ConfigurationManager.AppSettings["RabbitUserName"];
        private static readonly string RabbitPassword = ConfigurationManager.AppSettings["RabbitPassword"];

        static void Main(string[] args)
        {
            ConsoleAppHelper.PrintHeader("Header.txt");

            RunMassTransitReceiverWithRabbit();
        }

        private static void RunMassTransitReceiverWithRabbit()
        {

            var rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                var rabbitMqHost = rabbit.Host(new Uri(RabbitMqAddress), settings =>
                {
                    settings.Password(RabbitUsername);
                    settings.Username(RabbitPassword);
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, RabbitMqQueue, conf =>
                {
                    conf.Consumer<RegisterDatabaseConsumer>();
                });
            });

            rabbitBusControl.Start();


            Timer timer = new Timer(5000);
            timer.Elapsed += TimerTick;
            timer.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            timer.Stop();
            rabbitBusControl.Stop();
        }

        static void TimerTick(Object obj, ElapsedEventArgs e)
        {
          Monitor();
        }

        static void Monitor()
        {
            var previous = GetCurrentFootprint("PDM-LT-RAGANM", "messaging_spike");

            var connDetails = new DatabaseConnectionDetails
            {
                Server = "PDM-LT-RAGANM", Database = "messaging_spike", User = "sa", Password = "Mpxzpass1"
            };
            var current = Helper.GetSchemaInfo(connDetails);

            previous.Tables= previous.Tables.OrderBy(o => o.Name).ToList();
            current.Tables= current.Tables.OrderBy(o => o.Name).ToList();

            string json1 = JsonConvert.SerializeObject(previous);
            string json2 = JsonConvert.SerializeObject(current);

            TableComparer.Compare(json1, json2);

            var x = 2;
        }


        static Schema GetCurrentFootprint(string server, string database)
        {
            var schema = new Schema();
            var subPath = $@"C:\dev\Stores\SchemaScanner\{server}\{database}";

            var files = Directory.GetFiles(subPath, "*.txt", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                using (StreamReader x = File.OpenText(file))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    var moo = (Table)serializer.Deserialize(x, typeof(Table));

                    schema.Tables.Add(moo);
                }
            }
            
            return schema;
        }
    }
}
