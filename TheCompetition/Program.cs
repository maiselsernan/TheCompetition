using System;

namespace TheCompetition
{
    class Program
    {
        static void Main()
        {
            try
            {
                var i = 0;
                foreach (var winner in new Competition().GetWinners())
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

