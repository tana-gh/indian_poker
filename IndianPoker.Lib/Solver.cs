using System;
using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib
{
    public class Solver
    {
        public IEnumerable<(string playerName, int cardNumber)> NameAndNumbers { get; private set; }

        private IEnumerable<_Player> _Players { get; }

        public Solver(IEnumerable<int> allNumbers, IEnumerable<int> deckNumbers, IEnumerable<string> playerNames)
        {
            var deck     = new _Deck(allNumbers, deckNumbers);
            var cards    = deck.DeckCards.Take(playerNames.Count());
            var allCards = deck.SortedCards;
            
            _Players = _CreatePlayers(playerNames, cards, allCards);
        }

        private IEnumerable<_Player> _CreatePlayers(IEnumerable<string> playerNames, IEnumerable<_Card> cards, IEnumerable<_Card> allCards)
        {
            NameAndNumbers = playerNames.Zip(cards, (n, c) => (n, c.Number))
                                        .ToArray();

            IEnumerable<_VisibleCard> getVisibleCards(_Card card)
            {
                return playerNames.Zip(cards, (n, c) => (n, c))
                                  .Where(x => x.c != card)
                                  .Select(x => new _VisibleCard(x.n, x.c))
                                  .OrderBy(x => x.Card.Number)
                                  .ToArray();
            }

            var visibleCards = cards.Select(getVisibleCards);
            var nameAndCards = playerNames.Zip(visibleCards, (n, c) => (n, c));

            return nameAndCards.Select(x => new _Player(x.n, x.c, allCards))
                               .ToArray();
        }

        public IEnumerable<PlayerAnswer> Solve()
        {
            var turnCount = _Players.Count() + 1;
            return _PlayAllTurns(_Players, turnCount).ToArray();
        }

        private IEnumerable<PlayerAnswer> _PlayAllTurns(IEnumerable<_Player> players, int turnCount)
        {
            for (var i = 0; i < turnCount; i++)
            {
                foreach (var p in players)
                {
                    if (i == turnCount - 1 && p == players.Last())
                    {
                        break;
                    }

                    var answer = p.SendAnswer();

                    yield return answer;

                    if (answer.Value != AnswerValue.Unknown)
                    {
                        yield break;
                    }

                    foreach (var pp in players)
                    {
                        pp.ReceiveAnswer(answer);
                    }
                }
            }

            yield return new PlayerAnswer(null, AnswerValue.Infinite);
        }
    }
}
