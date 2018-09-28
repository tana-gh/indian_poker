using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib.Utils
{
    public static class Combination
    {
        public static IEnumerable<IEnumerable<int>> Generate(IEnumerable<int> numbers, int count)
        {
            return _AllCombination(numbers.ToArray(), new int[count], 0, 0).ToArray();
        }

        private static IEnumerable<IEnumerable<int>> _AllCombination(int[] src, int[] dst, int srcIndex, int dstIndex)
        {
            for (var i = srcIndex; i < src.Length; i++)
            {
                dst[dstIndex] = src[i];

                if (dstIndex < dst.Length - 1)
                {
                    foreach (var x in _AllCombination(src, dst, i + 1, dstIndex + 1))
                    {
                        yield return x;
                    }
                }
                else
                {
                    yield return (int[])dst.Clone();
                }
            }
        }
    }
}
