using System;
using Microsoft.Extensions.Configuration;

namespace TheCompetition
{
    class Program
    {
        private static string _startPath;
        private static string _endPath;
        static void Main()
        {
            try
            {
                InitPaths();

                Competition competition = new Competition();
                competition.ReadLog(_startPath , State.Start);
                competition.ReadLog(_endPath , State.Finish);
                competition.CalculateDiff();
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

        private static void InitPaths()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            _startPath = configuration["START_PATH"];
            if (string.IsNullOrEmpty(_startPath))
            {
                throw new Exception("Missing start log path");
            }

            _endPath = configuration["END_PATH"];
            if (string.IsNullOrEmpty(_endPath))
            {
                throw new Exception("Missing end log path");
            }
        }
    }
}

