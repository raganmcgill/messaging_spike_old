using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Timers;
using helpers;
using hospital.consumers;
using message_types.commands;
using MassTransit;
using Newtonsoft.Json;

namespace hospital
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

                sbc.ReceiveEndpoint(host, "Heartbeat", ep =>
                {
                    ep.Consumer(() => new HeartbeatConsumer());
                });
                
            });
            _rabbitBusControl.Start();


            Timer timer = new Timer(2000);
            timer.Elapsed += TimerTick;
            timer.Start();
            
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            timer.Stop();

            _rabbitBusControl.Stop();
        }

        static void TimerTick(Object obj, ElapsedEventArgs e)
        {
            ConsoleAppHelper.PrintHeader("Header.txt");

            GetStatuses();
        }

        public static void PrintTL(string file, ConsoleColor colour)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.White;
            ConsoleAppHelper.PrintHeader("Header.txt");

            Console.ForegroundColor = colour;

            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(file);

                //Read the first line of text
                line = sr.ReadLine();

                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the lie to console window
                    Console.WriteLine(line);
                    //Read the next line
                    line = sr.ReadLine();
                }
                Console.WriteLine(string.Empty);

                //close the file
                sr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException());
            }
            finally
            {
                ///Console.WriteLine("Executing finally block.");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        static void GetStatuses()
        {
            DateTime restartTime;

            string root = $@"C:\dev\Stores\hospital\";

            DirectoryInfo rootInfo = new DirectoryInfo(root);

            if (rootInfo.Exists)
            {
                var files = rootInfo.GetFiles();

                foreach (var fileInfo in files)
                {
                    var item = JsonConvert.DeserializeObject<Beat>(System.IO.File.ReadAllText(fileInfo.FullName));

                    if (item.Age <= 5)
                    {
                        PrintTL("TL_Green.txt", ConsoleColor.Green);
                    }
                    else if (item.Age > 5 && item.Age <= 15)
                    {
                        PrintTL("TL_Amber.txt", ConsoleColor.Yellow);
                    }
                    else
                    {
                        PrintTL("TL_Red.txt", ConsoleColor.Red);
//                        if (item.Age > 20)
//                        {
//                            Process.Start(item.Path);
//                        }
                    }

                    //                    if (item.Age >20 && !ProgramIsRunning(item.Path))
                    //                    {
                    //                        Process.Start(item.Path);
                    //                    }

                }
            }
            
        }

        public class Beat : Heartbeat
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public DateTime DateTime { get; set; }

            public double Age
            {
                get
                {
                    var diffInSeconds = (DateTime.Now - DateTime).TotalSeconds;
                    return diffInSeconds;
                }
            }
        }
    }
}
