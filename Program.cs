using System;
using System.Collections.Generic;
using System.IO;
using AdditionApi;
using AdditionApi.Models;
using Microsoft.Extensions.Configuration;

namespace AdditionApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration configuration = builder.Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            Database.CreateAndSeedDatabase(connectionString);

            Console.WriteLine("Reading data from the database...\n");

            List<string> results = Database.GetAllCalculations(connectionString);

            if (results.Count == 0)
                Console.WriteLine("No records found in the database.");
            else
                foreach (var result in results)
                    Console.WriteLine(result);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
