using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianPoker.Lib.Utils
{
    // ゲーム情報を可視化する
    public static class Visualizer
    {
        // 与えられたゲーム情報を文字列に変換
        // nameAndNumbers: プレイヤーとカードの組み合わせ列
        // answers: Solverの生成した答え列
        // return : 変換結果
        public static string ToString(IEnumerable<(string playerName, int cardNumber)> nameAndNumbers, IEnumerable<PlayerAnswer> answers)
        {
            var builder = new StringBuilder();

            // プレイヤー・カード情報
            builder.Append("<");

            var nnCount = nameAndNumbers.Count();

            foreach (var (p, c, i) in nameAndNumbers.Select((x, i) => (x.playerName, x.cardNumber, i)))
            {
                builder.Append($"{p}={c}");

                if (i < nnCount - 1)
                {
                    builder.Append(", ");
                }
            }
            
            builder.Append("> ");

            // 答え情報
            var ansCount = answers.Count();

            foreach (var (a, i) in answers.Select((a, i) => (a, i)))
            {
                builder.Append(a);

                if (i < ansCount - 1)
                {
                    builder.Append(", ");
                }
            }

            // 検証結果
            var validation = Validator.Validate(nameAndNumbers, answers);
            var result     = validation ? "OK" : "NG";
            builder.Append($" : {result}");

            return builder.ToString();
        }
    }
}
