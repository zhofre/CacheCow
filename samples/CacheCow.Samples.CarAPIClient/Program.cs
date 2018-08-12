using CacheCow.Client;
using System;
using System.Threading.Tasks;

namespace CacheCow.Samples.CarAPIClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CarAPI Client");

            var client = ClientExtensions.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5123");

            var p = new ConsoleMenu(client);

            Task.Run(async () => await p.Menu()).Wait();
        }
    }
}
