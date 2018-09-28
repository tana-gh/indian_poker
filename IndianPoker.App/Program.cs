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
            var cardCount   = 7;
            var playerCount = 3;

            var allNumbers = Enumerable.Range(0, cardCount).ToArray();
            var names      = PlayerNames.Generate(playerCount);
            var combs      = Combination.Generate(allNumbers, playerCount);

            foreach (var c in combs)
            {
                var perms = Permutation.Generate(c);

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
}
