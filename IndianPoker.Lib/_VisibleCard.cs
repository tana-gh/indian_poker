
namespace IndianPoker.Lib
{
    // 見えているカード
    internal struct _VisibleCard
    {
        // カードを所持するプレイヤーの名前
        public string PlayerName { get; }

        // 見えているカード
        public _Card Card { get; }

        // playerName: カードを所持するプレイヤーの名前
        // card: 見えているカード
        public _VisibleCard(string playerName, _Card card)
        {
            PlayerName = playerName;
            Card       = card;
        }
    }
}
