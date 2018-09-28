using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib.Utils
{
    public static class Validator
    {
        public static bool? Validate(IEnumerable<(string playerName, int cardNumber)> nameAndNumbers, IEnumerable<PlayerAnswer> answers)
        {
            var lastAnswer = answers.Last();
            
            if (lastAnswer.Value == AnswerValue.Unknown)
            {
                return false;
            }
            else if (lastAnswer.Value == AnswerValue.Infinite)
            {
                return null;
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
