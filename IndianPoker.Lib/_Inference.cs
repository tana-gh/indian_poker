using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib
{
    internal class _Inference
    {
        private enum _InferenceValue
        {
            Available,
            Used,
            Impossible
        }

        private struct _CardInfo
        {
            public int Index { get; }
            public _Card Card { get; }
            public string PlayerName { get; set; }
            public _InferenceValue Value { get; set; }

            public _CardInfo(int index, _Card card, string playerName, _InferenceValue value)
            {
                Index      = index;
                Card       = card;
                PlayerName = playerName;
                Value      = value;
            }
        }

        public PlayerAnswer Answer { get; set; }

        private int _Depth { get; }
        private string _PlayerName { get; }
        private _CardInfo[] _Table { get; }
        private IEnumerable<_Inference> _Children { get; }

        public _Inference(string playerName, IEnumerable<_VisibleCard> visibleCards, IEnumerable<_Card> allCards)
        {
            _Depth = 0;

            _PlayerName = playerName;
            _Table      = _CreateTable(visibleCards, allCards);

            Answer    = _GetAnswer();
            _Children = _CreateLowerInferences().Concat(_CreateUpperInferences())
                                                .ToArray();
        }

        private _Inference(string playerName, _CardInfo[] table, int depth)
        {
            _Depth = depth;

            _PlayerName = playerName;
            _Table      = table;

            if (depth <= table.Count(x => x.Value == _InferenceValue.Used))
            {
                Answer    = _GetAnswer();
                _Children = _CreateLowerInferences().Concat(_CreateUpperInferences())
                                                    .ToArray();
            }
            else
            {
                Answer    = new PlayerAnswer(playerName, AnswerValue.Infinite);
                _Children = new _Inference[0];
            }
        }

        private _CardInfo[] _CreateTable(IEnumerable<_VisibleCard> visibleCards, IEnumerable<_Card> allCards)
        {
            return allCards.GroupJoin(visibleCards, c => c.Number, v => v.Card.Number, (c, vs) => (c, v : vs.Cast<_VisibleCard?>().FirstOrDefault()))
                           .Select((x, i) => new _CardInfo(i, x.c, x.v?.PlayerName, x.v == null ? _InferenceValue.Available : _InferenceValue.Used))
                           .ToArray();
        }

        private PlayerAnswer _GetAnswer()
        {
            var count = _Table.Count();

            var lowers_available = _Table          .TakeWhile(x => x.Value != _InferenceValue.Used);
            var uppers_available = _Table.Reverse().TakeWhile(x => x.Value != _InferenceValue.Used);
            
            var lowers_used = _Table          .TakeWhile(x => x.Value != _InferenceValue.Available);
            var uppers_used = _Table.Reverse().TakeWhile(x => x.Value != _InferenceValue.Available);

            var lowers_isUsed = lowers_used.Any(x => x.Value == _InferenceValue.Used);
            var uppers_isUsed = uppers_used.Any(x => x.Value == _InferenceValue.Used);

            if (lowers_available.Count() + uppers_used.Count() == count)
            {
                return new PlayerAnswer(_PlayerName, AnswerValue.Min);
            }
            else if (lowers_used.Count() + uppers_available.Count() == count)
            {
                return new PlayerAnswer(_PlayerName, AnswerValue.Max);
            }
            else if (lowers_isUsed && uppers_isUsed)
            {
                return new PlayerAnswer(_PlayerName, AnswerValue.Mid);
            }
            else
            {
                return new PlayerAnswer(_PlayerName, AnswerValue.Unknown);
            }
        }

        private IEnumerable<_Inference> _CreateLowerInferences()
        {
            var availables = _Table.Reverse()
                                   .SkipWhile(x => x.Value == _InferenceValue.Available)
                                   .Where(x => x.Value == _InferenceValue.Available);
            var used = _Table.Last(x => x.Value == _InferenceValue.Used);

            return _CreateInferences(availables, used);
        }

        private IEnumerable<_Inference> _CreateUpperInferences()
        {
            var availables = _Table.SkipWhile(x => x.Value == _InferenceValue.Available)
                                   .Where(x => x.Value == _InferenceValue.Available);
            var used = _Table.First(x => x.Value == _InferenceValue.Used);

            return _CreateInferences(availables, used);
        }

        private IEnumerable<_Inference> _CreateInferences(IEnumerable<_CardInfo> availables, _CardInfo used)
        {
            if (Answer.Value != AnswerValue.Unknown)
            {
                yield break;
            }

            foreach (var a in availables)
            {
                var cloned = (_CardInfo[])_Table.Clone();

                _SetTableValue(cloned, a.Index, _PlayerName, _InferenceValue.Used);
                _SetTableValue(cloned, used.Index, null, _InferenceValue.Available);

                foreach (var i in cloned.Where(x => x.Value == _InferenceValue.Impossible))
                {
                    _SetTableValue(cloned, i.Index, null, _InferenceValue.Available);
                }

                yield return new _Inference(used.PlayerName, cloned, _Depth + 1);
            }
        }

        public void Update(PlayerAnswer received)
        {
            if (Answer.Value != AnswerValue.Unknown)
            {
                return;
            }

            foreach (var c in _Children)
            {
                c.Update(received);

                if (c.Answer.Value != AnswerValue.Unknown && c.Answer.Value != AnswerValue.Infinite)
                {
                    var me = c._Table.First(x => x.PlayerName == _PlayerName);
                    _SetTableValue(_Table, me.Index, _PlayerName, _InferenceValue.Impossible);
                }
            }

            if (received.PlayerName == _PlayerName)
            {
                Answer = _GetAnswer();
            }
        }

        private void _SetTableValue(_CardInfo[] table, int index, string playerName, _InferenceValue value)
        {
            table[index].PlayerName = playerName;
            table[index].Value      = value;
        }
    }
}
