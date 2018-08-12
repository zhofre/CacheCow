using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace CacheCow.Samples.CarAPI
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("CarAPI starting...");
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
               .UseUrls("http://localhost:5123")
               .Build();
    }
}
