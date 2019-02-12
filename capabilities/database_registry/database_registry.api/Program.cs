using System;
using System.Net.Http;
using helpers;
using Microsoft.Owin.Hosting;

namespace database_registry.api
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(80, 20);
            ConsoleAppHelper.PrintHeader("Header.txt");

            var baseAddress = "http://localhost:9111/api/registry/";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                HttpClient client = new HttpClient();

                Console.WriteLine(string.Empty);
                Console.WriteLine($"The api is live on {baseAddress}");
                Console.ReadLine();
            }
        }
    }
}
