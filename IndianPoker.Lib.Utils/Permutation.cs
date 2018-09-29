using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib.Utils
{
    public static class Permutation
    {
        // 順列を生成
        // numbers: 全体集合
        // return : 全順列
        public static IEnumerable<IEnumerable<int>> Generate(IEnumerable<int> numbers)
        {
            var array = numbers.ToArray();
            
            return _Heap(array, array.Length);
        }

        // Heap's algorithm により順列を生成
        // array : 記憶領域
        // n     : 交換位置
        // return: 全順列
        private static IEnumerable<IEnumerable<int>> _Heap(int[] array, int n)
        {
            if (n == 1)
            {
                yield return (int[])array.Clone();
                yield break;
            }
            
            for (var i = 0; i < n; i++)
            {
                foreach (var a in _Heap(array, n - 1))
                {
                    yield return a;
                }

                if (n % 2 == 0)
                {
                    (array[i], array[n - 1]) = (array[n - 1], array[i]);
                }
                else
                {
                    (array[0], array[n - 1]) = (array[n - 1], array[0]);
                }
            }
        }
    }
}
