using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib
{
    internal class _Deck
    {
        public IEnumerable<_Card> DeckCards { get; }
        public IEnumerable<_Card> SortedCards { get; }

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
