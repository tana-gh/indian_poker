using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib.Utils
{
    // Solverの結果を検証する
    public static class Validator
    {
        // 与えられたプレイヤー・カード情報と比較し、答えが正しいか判定する
        // nameAndNumbers: プレイヤーとカードの組み合わせ列
        // answers: Solverの生成した答え列
        // return : 検証結果
        public static bool Validate(IEnumerable<(string playerName, int cardNumber)> nameAndNumbers, IEnumerable<PlayerAnswer> answers)
        {
            var lastAnswer = answers.Last();
            
            if (lastAnswer.Value == AnswerValue.Unknown)
            {
                return false;
            }

            var validatee = nameAndNumbers.First(x => x.playerName == lastAnswer.PlayerName).cardNumber;
            var min = nameAndNumbers.Min(x => x.cardNumber);
            var max = nameAndNumbers.Max(x => x.cardNumber);

            return lastAnswer.Value == AnswerValue.Min && validatee == min ||
                   lastAnswer.Value == AnswerValue.Max && validatee == max ||
                   lastAnswer.Value == AnswerValue.Mid && min < validatee && validatee < max;
        }
    }
}
