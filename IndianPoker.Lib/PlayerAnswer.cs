
namespace IndianPoker.Lib
{
    public struct PlayerAnswer
    {
        public string PlayerName { get; }
        public AnswerValue Value { get; }

        public PlayerAnswer(string playerName, AnswerValue value)
        {
            PlayerName = playerName;
            Value      = value;
        }

        public override string ToString()
        {
            switch (Value)
            {
                case AnswerValue.Unknown:
                    return $"{PlayerName}=>?";

                case AnswerValue.Min:
                    return $"{PlayerName}=>MIN";

                case AnswerValue.Mid:
                    return $"{PlayerName}=>MID";

                case AnswerValue.Max:
                    return $"{PlayerName}=>MAX";

                case AnswerValue.Infinite:
                    return "Infinite loop";

                default:
                    return "";
            }
        }
    }
}
