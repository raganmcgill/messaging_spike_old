﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using helpers;

namespace RabbitClearer
{
    class Program
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]

        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;



        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE);

            ConsoleAppHelper.PrintHeader("Header.txt");

            Cleanup();

            System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\dev\Stores\");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                Console.WriteLine($"Removing {dir.FullName}");
                dir.Delete(true);
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        public static void Cleanup()
        {
            var username = "guest";
            var password = "guest";
            var host = "localhost";
            var virtualHost = Uri.EscapeDataString("/"); // %2F is the name of the default virtual host

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password),
            };
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(string.Format(CultureInfo.InvariantCulture, "http://{0}:15672/", host))
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var exchangeDeletionTasks = DeleteExchanges(httpClient, virtualHost);
            var queueDeletionTasks = DeleteQueues(httpClient, virtualHost);
            var cleanupTasks = exchangeDeletionTasks.Concat(queueDeletionTasks).ToArray();
            Task.WhenAll(cleanupTasks).Wait();

            foreach (var cleanup in cleanupTasks)
            {
                var responseMessage = cleanup.Result;
                try
                {
                    responseMessage.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException)
                {
                    var requestMessage = responseMessage.RequestMessage;
                    Console.WriteLine("Cleanup task failed for {0} {1}", requestMessage.Method, requestMessage.RequestUri);
                }
            }
        }

        static IEnumerable<Task<HttpResponseMessage>> DeleteExchanges(HttpClient httpClient, string virtualHost)
        {
            // Delete exchanges
            var exchangeResult = httpClient.GetAsync(string.Format(CultureInfo.InvariantCulture, "api/exchanges/{0}", virtualHost)).Result;
            exchangeResult.EnsureSuccessStatusCode();
            var allExchanges = JsonConvert.DeserializeObject<List<Exchange>>(exchangeResult.Content.ReadAsStringAsync().Result);
            var exchanges = FilterAllExchangesByExcludingInternalTheDefaultAndAmq(allExchanges);

            var exchangeDeletionTasks = new List<Task<HttpResponseMessage>>(exchanges.Count);
            exchangeDeletionTasks.AddRange(exchanges.Select(exchange =>
            {
                var deletionTask = httpClient.DeleteAsync(string.Format(CultureInfo.InvariantCulture, "/api/exchanges/{0}/{1}", virtualHost, exchange.Name));
                deletionTask.ContinueWith((t, o) => { Console.WriteLine("Deleted exchange {0}.", exchange.Name); }, null, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
                deletionTask.ContinueWith((t, o) => { Console.WriteLine("Failed to delete exchange {0}.", exchange.Name); }, null, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
                return deletionTask;
            }));
            return exchangeDeletionTasks;
        }

        static IEnumerable<Task<HttpResponseMessage>> DeleteQueues(HttpClient httpClient, string virtualHost)
        {
            var queueResult = httpClient.GetAsync(string.Format(CultureInfo.InvariantCulture, "api/queues/{0}", virtualHost)).Result;
            queueResult.EnsureSuccessStatusCode();
            var queues = JsonConvert.DeserializeObject<List<Queue>>(queueResult.Content.ReadAsStringAsync().Result);

            var queueDeletionTasks = new List<Task<HttpResponseMessage>>(queues.Count);
            queueDeletionTasks.AddRange(queues.Select(queue =>
            {
                var deletionTask = httpClient.DeleteAsync(string.Format(CultureInfo.InvariantCulture, "/api/queues/{0}/{1}", virtualHost, queue.Name));
                deletionTask.ContinueWith((t, o) => { Console.WriteLine("Deleted queue {0}.", queue.Name); }, null, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
                deletionTask.ContinueWith((t, o) => { Console.WriteLine("Failed to delete queue {0}.", queue.Name); }, null, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
                return deletionTask;
            }));
            return queueDeletionTasks;
        }

        private class Queue
        {
            public string Name { get; set; }
        }

        static List<Exchange> FilterAllExchangesByExcludingInternalTheDefaultAndAmq(IEnumerable<Exchange> allExchanges)
        {
            return (from exchange in allExchanges
                    let isInternal = exchange.Internal
                    let name = exchange.Name.ToLowerInvariant()
                    where !isInternal  // we should never delete rabbits internal exchanges
                    where !name.StartsWith("amq.") // amq.* we shouldn't remove
                    where name.Length > 0 // the default exchange which can't be deleted has a Name=string.Empty
                    select exchange)
                .ToList();
        }

        private class Exchange
        {
            public string Name { get; set; }
            public bool Internal { get; set; }
        }
    }
}
