
namespace IndianPoker.Lib
{
    internal struct _VisibleCard
    {
        public string PlayerName { get; }
        public _Card Card { get; }

        public _VisibleCard(string playerName, _Card card)
        {
            PlayerName = playerName;
            Card       = card;
        }
    }
}
