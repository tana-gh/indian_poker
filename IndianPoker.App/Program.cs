using System;
using System.Linq;
using IndianPoker.Lib;
using IndianPoker.Lib.Utils;

namespace IndianPoker.App
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: dotnet run -p <project> -- <card-count> <player-count>");
                return;
            }

            var cardCount   = int.Parse(args[0]);
            var playerCount = int.Parse(args[1]);

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
    }
}
