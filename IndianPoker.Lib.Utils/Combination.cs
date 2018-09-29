using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib.Utils
{
    public static class Combination
    {
        // 組み合わせ列を生成
        // numbers: 全体集合
        // count  : 組み合わせの元数
        // return : 全組み合わせ
        public static IEnumerable<IEnumerable<int>> Generate(IEnumerable<int> numbers, int count)
        {
            return _AllCombination(numbers.ToArray(), new int[count], 0, 0);
        }

        // 再帰的に組み合わせ列を生成
        // src: 全体集合
        // dst: 生成中の組み合わせの記憶領域
        // srcIndex: この添字以降の要素を対象とする
        // dstIndex: 現在の記憶領域の添字
        // return  : 全組み合わせ
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
