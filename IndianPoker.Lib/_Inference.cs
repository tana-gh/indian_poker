using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib
{
    // 推論に関する情報
    internal class _Inference
    {
        // メモ化に使用するキー
        private struct _MemoKey
        {
            public string TargetPlayerName { get; }
            public IEnumerable<_VisibleCard> VisibleCards { get; }
            public int OrderIndex { get; }

            public _MemoKey(string targetPlayerName, IEnumerable<_VisibleCard> visibleCards, int orderIndex)
            {
                TargetPlayerName = targetPlayerName;
                VisibleCards     = visibleCards;
                OrderIndex       = orderIndex;
            }
        }

        // _MemoKeyの等値比較子
        private class _MemoKeyEqualityComparer : IEqualityComparer<_MemoKey>
        {
            public static _MemoKeyEqualityComparer Instance { get; } = new _MemoKeyEqualityComparer();

            public bool Equals(_MemoKey x, _MemoKey y)
            {
                if (x.OrderIndex != y.OrderIndex || x.TargetPlayerName != y.TargetPlayerName)
                {
                    return false;
                }

                return x.VisibleCards.Zip(y.VisibleCards, (xx, yy) => xx.Card.Number == yy.Card.Number)
                                     .All(z => z);
            }

            public int GetHashCode(_MemoKey obj)
            {
                var sum = obj.VisibleCards.Select((x, i) => x.Card.Number * (i + 1))
                                          .Sum();
                
                unchecked
                {
                    return obj.TargetPlayerName.GetHashCode() + (sum + obj.OrderIndex).GetHashCode();
                }
            }
        }

        // 推論対象のプレイヤーの名前
        private string _PlayerName { get; }

        // プレイヤーに見えているカード列
        private IEnumerable<_VisibleCard> _VisibleCards { get; }

        // 全カード列
        private IEnumerable<_Card> _AllCards { get; }

        // プレイヤーの順序
        private _PlayerOrder _Order { get; }

        // _Inferメソッドのメモ
        private Dictionary<_MemoKey, PlayerAnswer> _Memo { get; }

        // playerName  : 推論対象のプレイヤーの名前
        // visibleCards: プレイヤーに見えているカード列
        // allCards    : 全カード列
        // order       : プレイヤーの順序
        public _Inference(string playerName, IEnumerable<_VisibleCard> visibleCards, IEnumerable<_Card> allCards, _PlayerOrder order)
        {
            _PlayerName   = playerName;
            _VisibleCards = visibleCards;
            _AllCards     = allCards;
            _Order        = order;

            _Memo = new Dictionary<_MemoKey, PlayerAnswer>(_MemoKeyEqualityComparer.Instance);
        }

        // 現在の順番に基づき、答えを導出する
        // orderIndex: _PlayerOrder上における現在の順番
        // return    : 答え
        public PlayerAnswer Infer(int orderIndex)
        {
            return _Infer(_PlayerName, _VisibleCards, orderIndex);
        }

        // 答えを導出する
        // targetPlayerName: 推論対象のプレイヤーの名前
        // visibleCards    : プレイヤーに見えているカード列
        // orderIndex      : _PlayerOrder上における現在の順番
        // return          : 答え
        public PlayerAnswer _Infer(string targetPlayerName, IEnumerable<_VisibleCard> visibleCards, int orderIndex)
        {
            var memoKey = new _MemoKey(targetPlayerName, visibleCards, orderIndex);

            // メモより取得
            if (_Memo.ContainsKey(memoKey))
            {
                return _Memo[memoKey];
            }

            // 自分のカードである可能性のあるカード列（あとで削除するためリストにする）
            var possibleCardList = _AllCards.Except(visibleCards.Select(x => x.Card))
                                            .ToList();

            // 前の手番が存在するなら
            if (orderIndex >= 1)
            {
                var prevOrderIndex = orderIndex - 1;
                var prevPlayerName = _Order.GetPlayerNames().ElementAt(prevOrderIndex);

                foreach (var p in possibleCardList.ToArray())
                {
                    // 前の手番を仮定する
                    var candidates = visibleCards.Concat(new[] { new _VisibleCard(targetPlayerName, p) })
                                                 .Where(x => x.PlayerName != prevPlayerName)
                                                 .OrderBy(x => x.PlayerName)
                                                 .ToArray();
                    
                    // 仮定を元に答えを導出する
                    var inferred = _Infer(prevPlayerName, candidates, prevOrderIndex);

                    // 答えの食い違い
                    if (inferred.Value != AnswerValue.Unknown)
                    {
                        possibleCardList.Remove(p);
                    }
                }
            }

            var answer = _GetAnswer(targetPlayerName, possibleCardList, visibleCards);
            _Memo.Add(memoKey, answer);

            return answer;
        }

        // 答えを取得する
        // targetPlayerName: 推論対象のプレイヤーの名前
        // possibleCards   : 対象プレイヤーのカードである可能性のあるカード列
        // visibleCards    : プレイヤーに見えているカード列
        // return          : 答え
        private PlayerAnswer _GetAnswer(string targetPlayerName, IEnumerable<_Card> possibleCards, IEnumerable<_VisibleCard> visibleCards)
        {
            var p_min = possibleCards.Min(x => x.Number);
            var p_max = possibleCards.Max(x => x.Number);
            var v_min = visibleCards .Min(x => x.Card.Number);
            var v_max = visibleCards .Max(x => x.Card.Number);
            
            if (p_max < v_min)
            {
                return new PlayerAnswer(targetPlayerName, AnswerValue.Min);
            }
            else if (v_max < p_min)
            {
                return new PlayerAnswer(targetPlayerName, AnswerValue.Max);
            }
            else if (v_min < p_min && p_max < v_max)
            {
                return new PlayerAnswer(targetPlayerName, AnswerValue.Mid);
            }
            else
            {
                return new PlayerAnswer(targetPlayerName, AnswerValue.Unknown);
            }
        }
    }
}
