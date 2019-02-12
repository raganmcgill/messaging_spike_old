using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using common_models;
using helpers;
using message_types.commands;
using message_types.events;
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
        private static readonly string RabbitUsername = ConfigurationManager.AppSettings["RabbitUserName"];
        private static readonly string RabbitPassword = ConfigurationManager.AppSettings["RabbitPassword"];
        private static IBusControl _rabbitBusControl;

        static Timer _checkIntervalTimer;
        static Timer _pulseIntervalTimer;

        static void Main(string[] args)
        {
            Console.SetWindowSize(80, 20);
            ConsoleAppHelper.PrintHeader("Header.txt");

            RunMassTransitReceiverWithRabbit();

            Monitor(null,null);
            Pulse(null,null);

            ConfigureCheckInterval();
            ConfigurePulseInterval();

            Console.ReadKey();

            _checkIntervalTimer.Stop();
            _pulseIntervalTimer.Stop();

            _rabbitBusControl.Stop();
        }

        private static void ConfigurePulseInterval()
        {
            _pulseIntervalTimer = new Timer(2000);
            _pulseIntervalTimer.Elapsed += Pulse;
            _pulseIntervalTimer.Start();
        }

        private static void ConfigureCheckInterval()
        {
            _checkIntervalTimer = new Timer(5000);
            _checkIntervalTimer.Elapsed += Monitor;
            _checkIntervalTimer.Start();
        }

        private static void RunMassTransitReceiverWithRabbit()
        {
            _rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                var rabbitMqHost = rabbit.Host(new Uri(RabbitMqAddress), settings =>
                {
                    settings.Username(RabbitUsername);
                    settings.Password(RabbitPassword);
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, "Registrations", conf =>
                {
                    conf.Consumer<RegisterDatabaseConsumer>();
                });
            });

            _rabbitBusControl.Start();
        }

        private static void Pulse(Object obj, ElapsedEventArgs e)
        {
            string fullName = Assembly.GetEntryAssembly().Location;

            Task<ISendEndpoint> sendEndpointTask = _rabbitBusControl.GetSendEndpoint(new Uri(string.Concat(RabbitMqAddress, "/Heartbeat")));

            ISendEndpoint sendEndpoint = sendEndpointTask.Result;

            Task sendTask = sendEndpoint.Send<Heartbeat>(new { Name = "Monitor", DateTime = DateTime.Now, Path = fullName });
        }

        static void Monitor(Object obj, ElapsedEventArgs e)
        {
            Console.Clear();
            ConsoleAppHelper.PrintHeader("Header.txt");
            Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss"));

            var storeLocation = $@"C:\dev\Stores\monitor\";

            var servers = RetrieveRegisteredServers(storeLocation);

            AnalyseServers(servers);
        }

        private static List<Server> RetrieveRegisteredServers(string storeLocation)
        {
            var serializer = new JsonSerializer();
            var servers = new List<Server>();

            var di = new DirectoryInfo(storeLocation);
            if (di.Exists)
            {
                foreach (var serverDirectoryInfo in di.GetDirectories())
                {
                    var server = new Server() { Name = serverDirectoryInfo.Name };

                    foreach (var databaseDirectoryInfo in serverDirectoryInfo.GetDirectories())
                    {
                        var database = new Database() { Name = databaseDirectoryInfo.Name };

                        foreach (var fileInfo in databaseDirectoryInfo.GetFiles())
                        {
                            if (!fileInfo.Name.ToLower().Contains("connection"))
                            {
                                using (StreamReader x = File.OpenText(fileInfo.FullName))
                                {
                                    var moo = (Table)serializer.Deserialize(x, typeof(Table));

                                    database.Tables.Add(moo);
                                }
                            }
                            else
                            {
                                using (StreamReader x = File.OpenText(fileInfo.FullName))
                                {
                                    database.ConnectionDetails =
                                        (ConnectionDetails)serializer.Deserialize(x, typeof(ConnectionDetails));
                                }
                            }
                        }

                        server.Databases.Add(database);
                    }

                    servers.Add(server);
                }
            }

            return servers;
        }

        private static void AnalyseServers(List<Server> servers)
        {
            foreach (var server in servers)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{server.Name}");

                AnalyseDatabases(server);
            }
        }

        private static void AnalyseDatabases(Server server)
        {
            foreach (var database in server.Databases)
            {
                if (database.ConnectionDetails != null)
                {
                    var current = Helper.GetSchemaInfo(database.ConnectionDetails);

                    var hasSchemaChanged = Compare(current, database);

                    if (hasSchemaChanged)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"     - {database.Name} :: Has changed");

                        StorageHelper.StoreDatabaseDefinition(database.ConnectionDetails, current.Tables, "monitor");

                        _rabbitBusControl.Publish<SchemaChanged>(new { Database = database});
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"     - {database.Name} :: Has not changed");
                    }
                }
            }
        }

        private static bool Compare(Database current, Database database1)
        {
            database1.Tables = database1.Tables.OrderBy(o => o.Name).ToList();
            current.Tables = current.Tables.OrderBy(o => o.Name).ToList();

            var json1 = JsonConvert.SerializeObject(database1);
            var json2 = JsonConvert.SerializeObject(current);

            var hasSchemaChanged = TableComparer.HasSchemaChanged(json1, json2);

            return hasSchemaChanged;
        }

    }
}
