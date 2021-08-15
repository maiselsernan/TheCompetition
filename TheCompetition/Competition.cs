using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TheCompetition
{
    public class Competition
    {
        private readonly Dictionary<string, Participant> _participants;
        private string _startPath;
        private string _endPath;
        public Competition()
        {
            _participants = new Dictionary<string, Participant>();
            InitPaths();

        }
        private void InitPaths()
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

        private void RunCompetition()
        {
            ReadLog(_startPath, State.Start);
            ReadLog(_endPath, State.Finish);
            CalculateDiff();
        }


        public IEnumerable<Participant> GetWinners()
        {
            RunCompetition();
            return _participants.Values.
                Where(o => o.StartTime != DateTime.MinValue && o.EndTime != DateTime.MinValue).
                OrderBy(x => x.Diff).Take(10);
        }

        private void ReadLog(string path, State state)
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
                            AddParticipantStart(tag, dateTime);
                            break;
                        case State.Finish:
                            AddParticipantEnd(tag, dateTime);
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

        private void AddParticipantEnd(string tag, DateTime dateTime)
        {
            if (_participants.TryGetValue(tag, out Participant participant))
            {
                if (dateTime > participant.StartTime)
                {
                    participant.EndTime = dateTime;
                }
            }
            else
            {
                _participants.Add(tag, new Participant { Tag = tag, EndTime = dateTime });
            }
        }

        private void AddParticipantStart(string tag, DateTime dateTime)
        {
            if (_participants.TryGetValue(tag, out Participant participant))
            {
                if (dateTime < participant.StartTime)
                {
                    participant.StartTime = dateTime;
                }
            }
            else
            {
                _participants.Add(tag, new Participant { Tag = tag, StartTime = dateTime });
            }
        }

        private void CalculateDiff()
        {
            foreach (var participant in _participants.Values.ToList())
            {
                participant.Diff = participant.EndTime - participant.StartTime;
            }
        }
    }

}
