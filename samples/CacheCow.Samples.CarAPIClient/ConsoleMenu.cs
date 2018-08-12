using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CacheCow.Client.Headers;
using CacheCow.Common.Helpers;

namespace CacheCow.Samples.CarAPIClient
{
    public class ConsoleMenu
    {
        private readonly HttpClient _client;
        private bool _verbose = false;

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
    - Press L to get the last item (default which is JSON)
    - Press X to get the last item in XML
    - Press C to create a new car and add to repo
    - Press U to update the last item (updates last modified)
    - Press O to update the last item outside API (updates last modified)
    - Press D to delete the last item
    - Press F to delete the first item
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
                            //await ListAllDataShaped();
                            break;
                        case 'C':
                        case 'c':
                            //await CreateNew();
                            break;
                        case 'U':
                        case 'u':
                            //await UpdateLast();
                            break;
                        case 'O':
                        case 'o':
                            //await UpdateLastOutsideApi();
                            break;
                        case 'D':
                        case 'd':
                            //await DeleteLast();
                            break;
                        case 'L':
                        case 'l':
                            //await GetLast();
                            break;
                        case 'X':
                        case 'x':
                            //await GetLastInXml();
                            break;
                        case 'F':
                        case 'f':
                            //await DeleteFirst();
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
                Console.WriteLine($"| {c.Id}\t| {c.NumberPlate}\t| {c.Owner}\t| {c.Brand}\t| {c.Color}\t| {c.Year}\t| {c.LastModified}\t|");
            }

            Console.WriteLine(new string('-', 113));
            Console.ResetColor();

            DumpHeaders(response);
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
