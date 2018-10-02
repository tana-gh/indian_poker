using System;
using System.Linq;
using IndianPoker.Lib;
using IndianPoker.Lib.Utils;

namespace IndianPoker.App.Multiple
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                _ShowUsage();
                return;
            }

            int cardCount;
            int playerCount;

            try
            {
                cardCount   = int.Parse(args[0]);
                playerCount = int.Parse(args[1]);
            }
            catch
            {
                _ShowUsage();
                return;
            }

            var allNumbers = Enumerable.Range(1, cardCount).ToArray();
            var names      = PlayerNames.Generate(playerCount);
            
            var perms = Combination.Generate(allNumbers, playerCount)
                                   .SelectMany(x => Permutation.Generate(x))
                                   .OrderBy(x => x, PermutationComparer.Instance)
                                   .ToArray();

            foreach (var p in perms)
            {
                var solver  = new Solver(allNumbers, p, names);
                var answers = solver.Solve();
                var result  = Visualizer.ToString(solver.NameAndNumbers, answers);
                Console.WriteLine(result);
            }
        }

        private static void _ShowUsage()
        {
            Console.WriteLine("Usage: dotnet run -p <project> -- <card-count> <player-count>");
        }
    }
}
