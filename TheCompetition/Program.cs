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
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
                // Dictionary<string, Participant> participants = new Dictionary<string, Participant>();
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

                Competition competition = new Competition();
                competition.ReadLog(startPath , State.Start);
                competition.ReadLog(endPath , State.Finish);
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
    }
}

