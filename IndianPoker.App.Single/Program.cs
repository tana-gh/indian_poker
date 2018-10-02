using System;
using System.Linq;
using IndianPoker.Lib;
using IndianPoker.Lib.Utils;

namespace IndianPoker.App.Single
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 2)
            {
                _ShowUsage();
                return;
            }

            int   cardCount;
            int   playerCount;
            int[] deckNumbers;

            try
            {
                cardCount   = int.Parse(args[0]);
                playerCount = int.Parse(args[1]);

                if (args.Length != playerCount + 2)
                {
                    _ShowUsage();
                    return;
                }

                deckNumbers = args.Skip(2)
                                  .Select(int.Parse)
                                  .ToArray();
            }
            catch
            {
                _ShowUsage();
                return;
            }

            var allNumbers = Enumerable.Range(1, cardCount).ToArray();
            var names      = PlayerNames.Generate(playerCount);

            var solver  = new Solver(allNumbers, deckNumbers, names);
            var answers = solver.Solve();
            var result  = Visualizer.ToString(solver.NameAndNumbers, answers);
            Console.WriteLine(result);
        }

        private static void _ShowUsage()
        {
            Console.WriteLine("Usage: dotnet run -p <project> -- <card-count> <player-count> <deck-list>");
        }
    }
}
