using System;

namespace TheCompetition
{
    class Participant
    {
        public string Tag { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Diff { get; set; }
    }
}