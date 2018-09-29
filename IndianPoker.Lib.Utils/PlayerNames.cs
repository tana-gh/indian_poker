using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib.Utils
{
    public static class PlayerNames
    {
        // プレイヤーの名前列を生成
        // count : プレイヤーの人数
        // return: 全プレイヤーの名前
        public static IEnumerable<string> Generate(int count)
        {
            return Enumerable.Range(0, count)
                             .Select(x => new string(_ToAlphabets(x).ToArray()))
                             .ToArray();
        }

        // 数値をアルファベット列に変換
        private static IEnumerable<char> _ToAlphabets(int n)
        {
            var c = _ToAlphabet(n % 26);
            var m = n / 26;

            if (m == 0)
            {
                yield return c;
                yield break;
            }

            foreach (var cc in _ToAlphabets(m - 1))
            {
                yield return cc;
            }

            yield return c;
        }

        // 数値をアルファベットに変換
        private static char _ToAlphabet(int n)
        {
            return (char)('A' + n);
        }
    }
}
