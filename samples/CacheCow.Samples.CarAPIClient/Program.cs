using CacheCow.Client;
using System;
using System.Net.Http;

namespace CacheCow.Samples.CarAPIClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CarAPI Client");

            var client = ClientExtensions.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5123");

            // todo: add console menu
        }
    }
}
