using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CacheCow.Client.Headers;
using CacheCow.Common.Helpers;

namespace CacheCow.Samples.CarAPIClient
{
    public class ConsoleMenu
    {
        private readonly HttpClient _client;
        private readonly Random _rnd = new Random();
        private bool _verbose = false;
        private readonly string[] _brands = new[] {"Audi", "BMW", "Citroën", "Jaguar", "Renault", "Seat"};
        private readonly string[] _colors = new[] { "RoyalBlue", "DarkRed", "PearlWhite", "CarbonBlack", "Antracite", "LightBlue" };
        private readonly string[] _owners = new[] { "John Doe", "Jean Doux", "Jan Doo", "Jane Doe", "Jeanne Doux", "Janneke Doo" };

        public ConsoleMenu(HttpClient client)
        {
            _client = client;
        }

        public async Task Menu()
        {
            while(true)
            {
                Console.WriteLine(
@"CacheCow CarAPI Sample - (ASP.NET Core Service and HttpClient)
    - Press A to list all cars
    - Press Z to list data shaped cars (number plate and owner)
    - Press L to get the last item
    - Press M to get the last item data shaped
    - Press C to create a new car and add to repo
    - Press U to update the first item (updates number plate, owner, color and last modified)
    - Press D to delete the last item
    - Press F to delete the car with id 1
    - Press V to toggle on/off verbose header dump
    - Press Q to exit
"
);
                try
                {
                    var key = Console.ReadKey(true);
                    switch (key.KeyChar)
                    {
                        case 'q':
                        case 'Q':
                            return;
                        case 'a':
                        case 'A':
                            await ListAll();
                            break;
                        case 'z':
                        case 'Z':
                            await ListAllDataShaped();
                            break;
                        case 'C':
                        case 'c':
                            await CreateNew();
                            break;
                        case 'U':
                        case 'u':
                            await UpdateFirst();
                            break;
                        case 'D':
                        case 'd':
                            await DeleteLast();
                            break;
                        case 'L':
                        case 'l':
                            await GetLast();
                            break;
                        case 'M':
                        case 'm':
                            await GetLastDataShaped();
                            break;
                        case 'F':
                        case 'f':
                            await DeleteFirst();
                            break;
                        case 'V':
                        case 'v':
                            Toggle();
                            break;
                        default:
                            // nothing
                            Console.WriteLine("Invalid option: " + key.KeyChar);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine(e.ToString());
                    Console.ResetColor();
                }
            }
        }

        public void Toggle()
        {
            _verbose = !_verbose;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(_verbose ? "Verbose toggle is ON" : "Verbose toggle is OFF");
            Console.WriteLine();
            Console.ResetColor();
        }

        public async Task ListAll()
        {
            var response = await _client.GetAsync("/api/cars");
            await response.WhatEnsureSuccessShouldHaveBeen();
            await response.Content.LoadIntoBufferAsync();
            WriteCacheCowHeader(response);
            Console.ForegroundColor = ConsoleColor.White;
            var cars = await response.Content.ReadAsAsync<CarAPI.Dto.LinkedResourceCollection<CarAPI.Dto.Car>>();

            Console.WriteLine(new string('-', 113));
            Console.WriteLine($"| Id\t| NumberPlate\t| Owner\t\t| Brand\t\t| Color\t\t| Year\t| Last Modified Date\t\t|");

            foreach (var c in cars.Content)
            {
                Console.WriteLine($"| {c.Id}\t| {c.NumberPlate}\t| {c.Owner}\t| {c.Brand,6}\t| {c.Color}\t| {c.Year}\t| {c.LastModified}\t|");
            }

            Console.WriteLine(new string('-', 113));
            Console.ResetColor();

            DumpHeaders(response);
        }

        public async Task ListAllDataShaped()
        {
            var response = await _client.GetAsync("/api/cars?fields=numberPlate,owner");
            await response.WhatEnsureSuccessShouldHaveBeen();
            await response.Content.LoadIntoBufferAsync();
            WriteCacheCowHeader(response);
            Console.ForegroundColor = ConsoleColor.White;
            var cars = await response.Content.ReadAsAsync<CarAPI.Dto.LinkedResourceCollection<CarAPI.Dto.Car>>();

            Console.WriteLine(new string('-', 73));
            Console.WriteLine($"| Id\t| NumberPlate\t| Owner\t\t| Last Modified Date\t\t|");

            foreach (var c in cars.Content)
            {
                Console.WriteLine($"| {c.Id}\t| {c.NumberPlate}\t| {c.Owner}\t| {c.LastModified}\t|");
            }

            Console.WriteLine(new string('-', 73));
            Console.ResetColor();

            DumpHeaders(response);
        }

        public async Task CreateNew()
        {
            var newCar = new CarAPI.Dto.CarForCreation
            {
                Brand = _brands[_rnd.Next(0, 6)],
                Color = _colors[_rnd.Next(0, 6)],
                NumberPlate = CreateNumberPlate(),
                Owner = _owners[_rnd.Next(0, 6)],
                Year = 1990 + _rnd.Next(0, 28)
            };
            var response = await _client.PostAsync("/api/cars", newCar, new JsonMediaTypeFormatter());
            WriteCacheCowHeader(response);
            await response.WhatEnsureSuccessShouldHaveBeen();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Location header: {response.Headers.Location}");
            Console.WriteLine();
            Console.ResetColor();

            DumpHeaders(response);
        }

        public async Task GetLast()
        {
            var response = await _client.GetAsync("/api/cars/last");
            await response.WhatEnsureSuccessShouldHaveBeen();
            WriteCacheCowHeader(response);
            Console.ForegroundColor = ConsoleColor.White;
            var c = await response.Content.ReadAsAsync<CarAPI.Dto.Car>();
            Console.WriteLine($"| {c.Id}\t| {c.NumberPlate}\t| {c.Owner}\t| {c.Brand,6}\t| {c.Color}\t| {c.Year}\t| {c.LastModified}\t|");
            Console.WriteLine();
            Console.ResetColor();
            DumpHeaders(response);
        }

        public async Task GetLastDataShaped()
        {
            var response = await _client.GetAsync("/api/cars/last?fields=numberPlate,owner");
            await response.WhatEnsureSuccessShouldHaveBeen();
            WriteCacheCowHeader(response);
            Console.ForegroundColor = ConsoleColor.White;
            var c = await response.Content.ReadAsAsync<CarAPI.Dto.Car>();
            Console.WriteLine($"| {c.Id}\t| {c.NumberPlate}\t| {c.Owner}\t| {c.Brand,6}\t| {c.Color}\t| {c.Year}\t| {c.LastModified}\t|");
            Console.WriteLine();
            Console.ResetColor();
            DumpHeaders(response);
        }

        public async Task UpdateFirst()
        {
            var updatedCar = new CarAPI.Dto.CarForManipulation
            {
                Color = _colors[_rnd.Next(0, 6)],
                NumberPlate = CreateNumberPlate(),
                Owner = _owners[_rnd.Next(0, 6)]
            };

            var response = await _client.PutAsync("/api/cars/1", updatedCar, new JsonMediaTypeFormatter());
            WriteCacheCowHeader(response);
            await response.WhatEnsureSuccessShouldHaveBeen();
            DumpHeaders(response);
        }

        public async Task DeleteLast()
        {
            var response = await _client.DeleteAsync("/api/cars/last");
            WriteCacheCowHeader(response);
            await response.WhatEnsureSuccessShouldHaveBeen();
            DumpHeaders(response);
        }

        public async Task DeleteFirst()
        {
            var response = await _client.DeleteAsync("/api/cars/1");
            WriteCacheCowHeader(response);
            await response.WhatEnsureSuccessShouldHaveBeen();
            DumpHeaders(response);
        }

        private string CreateNumberPlate()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(new []
                   {
                       chars[_rnd.Next(0, 26)],
                       chars[_rnd.Next(0, 26)],
                       chars[_rnd.Next(0, 26)]
                   })
                + "-"
                + _rnd.Next(0, 10)
                + _rnd.Next(0, 10)
                + _rnd.Next(0, 10);
        }

        public void DumpHeaders(HttpResponseMessage response)
        {
            if (!_verbose)
                return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"REQUEST:\r\n{response.RequestMessage.Headers.ToString()}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"RESPONSE:\r\n{response.Headers.ToString()}");
            if (response.Content != null)
            {
                Console.WriteLine($"RESPONSE CONTENT:\r\n{response.Content.Headers.ToString()}");
                Console.WriteLine();
            }
            Console.ResetColor();

        }

        static void WriteCacheCowHeader(HttpResponseMessage response)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Client: {response.Headers.GetCacheCowHeader()}");
            if (response.Headers.Contains("x-cachecow-server"))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                var tmp = response.Headers.GetValues("x-cachecow-server").FirstOrDefault() ?? "";
                Console.WriteLine($"Server: {tmp}");
            }

            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
