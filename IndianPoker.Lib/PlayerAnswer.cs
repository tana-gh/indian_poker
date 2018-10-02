
namespace IndianPoker.Lib
{
    // プレイヤーの答え
    public struct PlayerAnswer
    {
        // 答えを宣言したプレイヤーの名前（nullable）
        public string PlayerName { get; }

        // 答えの内容
        public AnswerValue Value { get; }

        // playerName: 答えを宣言したプレイヤーの名前（nullable）
        // value: 答えの内容
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

                default:
                    return "";
            }
        }
    }
}
