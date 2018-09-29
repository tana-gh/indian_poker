using System.Collections.Generic;
using System.Linq;
using Xunit;
using IndianPoker.Lib;
using IndianPoker.Lib.Utils;

namespace IndianPoker.Test
{
    public class SolverTest
    {
        [Theory]
        [MemberData(nameof(TestMain_Data), 8)] // カード数 9 以上は計算時間が膨大となる
        public void TestMain(int cardCount, int playerCount)
        {
            var allNumbers = Enumerable.Range(1, cardCount).ToArray();
            var names      = PlayerNames.Generate(playerCount);

            var perms = Combination.Generate(allNumbers, playerCount)
                                   .SelectMany(x => Permutation.Generate(x))
                                   .ToArray();

            foreach (var p in perms)
            {
                var solver  = new Solver(allNumbers, p, names);
                var answers = solver.Solve();
                
                var validation = Validator.Validate(solver.NameAndNumbers, answers);
                Assert.True(validation == null || validation.Value); // Infinite loop はチェック対象外
            }
        }

        public static IEnumerable<object[]> TestMain_Data(int count)
        {
            for (var i = 2; i < count; i++)
            {
                for (var j = 2; j <= i; j++)
                {
                    yield return new object[] { i, j };
                }
            }
        }
    }
}
