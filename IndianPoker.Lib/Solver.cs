using System;
using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib
{
    // ゲームを進行し、結果を生成
    public class Solver
    {
        // 参加プレイヤーと所持カード
        public IEnumerable<(string playerName, int cardNumber)> NameAndNumbers { get; private set; }

        // プレイヤー列
        private IEnumerable<_Player> _Players { get; }

        // allNumbers : 全カードの数字列
        // deckNumbers: プレイヤーのドローするカードの数字列（与えられた順）
        // playerNames: プレイヤーの名前列（与えられた順）
        public Solver(IEnumerable<int> allNumbers, IEnumerable<int> deckNumbers, IEnumerable<string> playerNames)
        {
            var allCount    = allNumbers .Count();
            var deckCount   = deckNumbers.Count();
            var playerCount = playerNames.Count();

            if (allCount    <  2 ||
                playerCount <  2 ||
                allCount    <  playerCount ||
                deckCount   != playerCount)
            {
                throw new ArgumentException();
            }

            var deck     = new _Deck(allNumbers, deckNumbers);
            var cards    = deck.DeckCards;
            var allCards = deck.SortedCards;
            
            _Players = _CreatePlayers(playerNames, cards, allCards);
        }

        // プレイヤー列を生成
        // playerNames: プレイヤーの名前列
        // cards      : プレイヤーのドローするカード列
        // allCards   : 全カード列
        // return     : プレイヤー列
        private IEnumerable<_Player> _CreatePlayers(IEnumerable<string> playerNames, IEnumerable<_Card> cards, IEnumerable<_Card> allCards)
        {
            // 公開用のプレイヤー・カード情報
            NameAndNumbers = playerNames.Zip(cards, (n, c) => (n, c.Number))
                                        .ToArray();

            // 自プレイヤーのカードから、他プレイヤーのカード列を生成
            // card: 自プレイヤーのカード
            IEnumerable<_VisibleCard> getVisibleCards(_Card card)
            {
                return playerNames.Zip(cards, (n, c) => (n, c))
                                  .Where(x => x.c != card)
                                  .Select(x => new _VisibleCard(x.n, x.c))
                                  .OrderBy(x => x.Card.Number)
                                  .ToArray();
            }

            // プレイヤー名と、そのプレイヤーが見えているカードを関連付け
            var visibleCards = cards.Select(getVisibleCards);
            var nameAndCards = playerNames.Zip(visibleCards, (n, c) => (n, c));

            // プレイヤー列を生成
            return nameAndCards.Select(x => new _Player(x.n, x.c, allCards))
                               .ToArray();
        }

        // ゲームを進行し、結果を生成
        // return: 答え列
        public IEnumerable<PlayerAnswer> Solve()
        {
            return _PlayAllTurns().ToArray();
        }

        // ゲームの進行
        // return   : 答え列
        private IEnumerable<PlayerAnswer> _PlayAllTurns()
        {
            for (;;)
            {
                foreach (var p in _Players)
                {
                    // 答えを宣言
                    var answer = p.SendAnswer();

                    yield return answer;

                    // Unknown以外の答えなら終了
                    if (answer.Value != AnswerValue.Unknown)
                    {
                        yield break;
                    }

                    // 答えを他プレイヤーに送信
                    foreach (var pp in _Players.Where(pp => pp != p))
                    {
                        pp.ReceiveAnswer(answer);
                    }
                }
            }
        }
    }
}
