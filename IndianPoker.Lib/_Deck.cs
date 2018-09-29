using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib
{
    // カードのデッキ
    internal class _Deck
    {
        // プレイヤーがドローするカード列
        public IEnumerable<_Card> DeckCards { get; }

        // ソート済みの全カード
        public IEnumerable<_Card> SortedCards { get; }

        // allNumber  : 全カードの数字列
        // deckNumbers: プレイヤーがドローするカードの数字列
        public _Deck(IEnumerable<int> allNumbers, IEnumerable<int> deckNumbers)
        {
            SortedCards = allNumbers.OrderBy(x => x)
                                    .Select(x => new _Card(x))
                                    .ToArray();
            DeckCards = deckNumbers.Join(SortedCards, d => d, c => c.Number, (d, c) => c)
                                   .ToArray();
        }
    }
}
