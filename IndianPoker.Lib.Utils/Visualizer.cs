using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndianPoker.Lib.Utils
{
    public static class Visualizer
    {
        public static string ToString(IEnumerable<(string playerName, int cardNumber)> nameAndNumbers, IEnumerable<PlayerAnswer> answers)
        {
            var builder = new StringBuilder();

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

            var ansCount = answers.Count();

            foreach (var (a, i) in answers.Select((a, i) => (a, i)))
            {
                builder.Append(a);

                if (i < ansCount - 1)
                {
                    builder.Append(", ");
                }
            }

            var validation = Validator.Validate(nameAndNumbers, answers);
            
            if (validation != null)
            {
                var result = validation.Value ? "OK" : "NG";
                builder.Append($" : {result}");
            }

            return builder.ToString();
        }
    }
}
