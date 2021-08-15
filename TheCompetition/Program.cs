using System;
using Microsoft.Extensions.Configuration;

namespace TheCompetition
{
    class Program
    {

        static void Main()
        {
            try
            {
                var paths = GetPaths();
                var competition = new Competition(paths.StartPath, paths.EndPath);
                var i = 0;
                foreach (var winner in competition.GetWinners())
                {
                    i++;
                    Console.WriteLine($"Place: {i}, Tag: {winner.Tag}, Time: {winner.Diff}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private static (string StartPath, string EndPath) GetPaths()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var startPath = configuration["START_PATH"];
            if (string.IsNullOrEmpty(startPath))
            {
                throw new Exception("Missing start log path");
            }

            var endPath = configuration["END_PATH"];
            if (string.IsNullOrEmpty(endPath))
            {
                throw new Exception("Missing end log path");
            }
            return (startPath, endPath);
        }
    }




}

