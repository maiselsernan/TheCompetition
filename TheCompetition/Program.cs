using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                Dictionary<string, Participant> participants = new Dictionary<string, Participant>();
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
                ReadLog(participants,startPath , State.Start);
                ReadLog(participants, endPath, State.Finish);

                foreach (var participant in participants.Values.ToList())
                {
                    participant.Diff = participant.EndTime - participant.StartTime;
                }

                var i = 0;
                foreach (var winner in GetWinners(participants))
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

        private static IEnumerable<Participant> GetWinners(Dictionary<string, Participant> participants)
        {
            return participants.Values.
                Where(o => o.StartTime != DateTime.MinValue && o.EndTime != DateTime.MinValue).
                OrderBy(x => x.Diff).Take(10);
        }

        private static void ReadLog(Dictionary<string, Participant> participants, string path, State state)
        {
            if (!File.Exists(path))
            {
                throw new Exception($"Missing {state} log File");
            }
            var startLines = File.ReadAllLines(path);
            foreach (var line in startLines)
            {
                try
                {
                    var tag = line.Substring(4, 12);
                    var timestampString = line.Substring(20, 12);

                    DateTime dateTime = DateTime.ParseExact(timestampString, "yyMMddHHmmss", null);
                    switch (state)
                    {
                        case State.Start:
                            AddParticipantStart(participants, tag, dateTime);
                            break;
                        case State.Finish:
                            AddParticipantEnd(participants, tag, dateTime);
                            break;
                        default:
                            continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void AddParticipantEnd(Dictionary<string, Participant> participants, string tag, DateTime dateTime)
        {
            if (participants.TryGetValue(tag, out Participant participant))
            {
                if (dateTime > participant.StartTime)
                {
                    participant.EndTime = dateTime;
                }
            }
            else
            {
                participants.Add(tag, new Participant { Tag = tag, EndTime = dateTime });
            }
        }

        private static void AddParticipantStart(Dictionary<string, Participant> participants, string tag, DateTime dateTime)
        {
            if (participants.TryGetValue(tag, out Participant participant))
            {
                if (dateTime < participant.StartTime)
                {
                    participant.StartTime = dateTime;
                }
            }
            else
            {
                participants.Add(tag, new Participant { Tag = tag, StartTime = dateTime });
            }
        }
    }
}

