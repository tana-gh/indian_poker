using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib
{
    // 推論情報
    internal class _Inference
    {
        // カードに対する、推論の内容
        private enum _InferenceValue
        {
            Available, // 可能性あり
            Used,      // 他プレイヤーが所持
            Impossible // 可能性なし
        }

        // 推論に必要なカードの情報
        private struct _CardInfo
        {
            // 推論テーブルにおける位置
            public int Index { get; }

            // 対象となるカード
            public _Card Card { get; }

            // カードに関連するプレイヤーの名前（nullable）
            public string PlayerName { get; set; }

            // 推論の内容
            public _InferenceValue Value { get; set; }

            // index: 推論テーブルにおける位置
            // card : 対象となるカード
            // playerName: カードに関連するプレイヤーの名前（nullable）
            // value: 推論値
            public _CardInfo(int index, _Card card, string playerName, _InferenceValue value)
            {
                Index      = index;
                Card       = card;
                PlayerName = playerName;
                Value      = value;
            }
        }

        // 現在の答え
        public PlayerAnswer Answer { get; set; }

        // 推論情報の階層の深度
        private int _Depth { get; }

        // 推論対象のプレイヤーの名前
        private string _PlayerName { get; }

        // 推論テーブル（カードの情報列）
        private _CardInfo[] _Table { get; }
        
        // この推論情報の子にあたる推論情報列
        private IEnumerable<_Inference> _Children { get; }

        // 階層のルートを生成する、パブリックコンストラクタ
        // playerName  : 推論対象のプレイヤーの名前
        // visibleCards: プレイヤーに見えているカード列
        // allCards    : 全カード列
        public _Inference(string playerName, IEnumerable<_VisibleCard> visibleCards, IEnumerable<_Card> allCards)
        {
            _Depth = 0;

            _PlayerName = playerName;
            _Table      = _CreateTable(visibleCards, allCards);

            var answer = _GetAnswer();
            _Children  = _CreateLowerInferences(answer).Concat(_CreateUpperInferences(answer))
                                                       .ToArray();
            
            // 現在の答えは、未更新のため上記 answer とはせず、Unknown とする
            Answer = new PlayerAnswer(playerName, AnswerValue.Unknown);
        }

        // 子要素を生成する、プライベートコンストラクタ
        // playerName: 推論対象のプレイヤーの名前
        // table: 推論テーブル
        // depth: 推論情報の階層の深度
        private _Inference(string playerName, _CardInfo[] table, int depth)
        {
            _Depth = depth;

            _PlayerName = playerName;
            _Table      = table;

            if (depth <= table.Count(x => x.Value == _InferenceValue.Used))
            {
                // 深度が全プレイヤー数未満（他プレイヤー数以下）の場合、子を生成
                var answer = _GetAnswer();
                _Children  = _CreateLowerInferences(answer).Concat(_CreateUpperInferences(answer))
                                                           .ToArray();

                // 現在の答えは、未更新のため上記 answer とはせず、Unknown とする
                Answer = new PlayerAnswer(playerName, AnswerValue.Unknown);
            }
            else
            {
                // 深度が全プレイヤー数の場合、子は生成しない
                Answer    = new PlayerAnswer(playerName, AnswerValue.Infinite);
                _Children = new _Inference[0];
            }
        }

        // 推論テーブルを生成
        // visibleCards: 見えているカード列
        // allCards: 全カード列
        // return  : 推論テーブル
        private _CardInfo[] _CreateTable(IEnumerable<_VisibleCard> visibleCards, IEnumerable<_Card> allCards)
        {
            // 全カード列に対し、見えているカード列を関連付け、その結果をもとにカード情報を生成
            return allCards.GroupJoin(visibleCards, c => c.Number, v => v.Card.Number, (c, vs) => (c, v : vs.Cast<_VisibleCard?>().FirstOrDefault()))
                           .Select((x, i) => new _CardInfo(i, x.c, x.v?.PlayerName, x.v == null ? _InferenceValue.Available : _InferenceValue.Used))
                           .ToArray();
        }

        // 現在の推論テーブルの状態から答えを取得
        // return: 答え
        private PlayerAnswer _GetAnswer()
        {
            var count = _Table.Count();

            // テーブル上下の Available または Impossible
            var lowers_available = _Table          .TakeWhile(x => x.Value != _InferenceValue.Used);
            var uppers_available = _Table.Reverse().TakeWhile(x => x.Value != _InferenceValue.Used);
            
            // テーブル上下の Used または Impossible
            var lowers_used = _Table          .TakeWhile(x => x.Value != _InferenceValue.Available);
            var uppers_used = _Table.Reverse().TakeWhile(x => x.Value != _InferenceValue.Available);

            // lowers_used および uppers_used に Used が含まれているか
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

        // 推論テーブル下部に対し推論を行ない、子要素列を生成する
        // answer: 現在の推論テーブルの状態から得られる答え
        // return: 推論結果の子要素列
        private IEnumerable<_Inference> _CreateLowerInferences(PlayerAnswer answer)
        {
            // 上部の Available を除外し、Available を取得
            var availables = _Table.Reverse()
                                   .SkipWhile(x => x.Value == _InferenceValue.Available)
                                   .Where(x => x.Value == _InferenceValue.Available);

            // 上部の Used
            var used = _Table.Last(x => x.Value == _InferenceValue.Used);

            return _CreateInferences(availables, used, answer);
        }

        // 推論テーブル上部に対し推論を行ない、子要素列を生成する
        // answer: 現在の推論テーブルの状態から得られる答え
        // return: 推論結果の子要素列
        private IEnumerable<_Inference> _CreateUpperInferences(PlayerAnswer answer)
        {
            // 下部の Available を除外し、Available を取得
            var availables = _Table.SkipWhile(x => x.Value == _InferenceValue.Available)
                                   .Where(x => x.Value == _InferenceValue.Available);

            // 下部の Used
            var used = _Table.First(x => x.Value == _InferenceValue.Used);

            return _CreateInferences(availables, used, answer);
        }

        // 推論を行ない、子要素列を生成する
        // availables: 自分の位置の候補列
        // used  : 子要素の対象プレイヤーのカード情報
        // answer: 現在の推論テーブルの状態から得られる答え
        // return: 推論結果の子要素列
        private IEnumerable<_Inference> _CreateInferences(IEnumerable<_CardInfo> availables, _CardInfo used, PlayerAnswer answer)
        {
            // 推論結果が確定していたら、子要素の推論はしない
            if (answer.Value != AnswerValue.Unknown)
            {
                yield break;
            }

            // 各候補位置について、子要素を生成
            foreach (var a in availables)
            {
                // 子要素用のテーブルを作成
                var cloned = (_CardInfo[])_Table.Clone();

                _SetTableValue(cloned, a.Index, _PlayerName, _InferenceValue.Used);  // 候補位置に自分を置く
                _SetTableValue(cloned, used.Index, null, _InferenceValue.Available); // 子要素の対象プレイヤー位置を取得可能にする

                // Impossible を取得可能にする
                foreach (var i in cloned.Where(x => x.Value == _InferenceValue.Impossible))
                {
                    _SetTableValue(cloned, i.Index, null, _InferenceValue.Available);
                }

                yield return new _Inference(used.PlayerName, cloned, _Depth + 1);
            }
        }

        // 他プレイヤーおよび自プレイヤーの宣言した結果をもとに、推論テーブルを更新する
        // received: 宣言された答え
        public void Update(PlayerAnswer received)
        {
            // 既に答えが確定していたら、推論しない
            if (Answer.Value != AnswerValue.Unknown)
            {
                return;
            }

            // 各子要素を更新する
            foreach (var c in _Children)
            {
                c.Update(received); // 子要素の更新

                // 答えが確定している場合（無限ループを除く）Impossible の設定
                if (c.Answer.Value != AnswerValue.Unknown && c.Answer.Value != AnswerValue.Infinite)
                {
                    // 子要素の推論テーブル中にある自分の位置を、自分の推論テーブル上で Impossible とする
                    var me = c._Table.First(x => x.PlayerName == _PlayerName);
                    _SetTableValue(_Table, me.Index, _PlayerName, _InferenceValue.Impossible);
                }
            }

            // 答えを宣言したプレイヤーが対象プレイヤーである場合、答えを更新
            if (received.PlayerName == _PlayerName)
            {
                Answer = _GetAnswer();
            }
        }

        // 推論テーブルの与えられた位置に値を設定
        // table: 推論テーブル
        // index: 位置
        // playerName: 対象プレイヤーの名前
        // value: 推論値
        private void _SetTableValue(_CardInfo[] table, int index, string playerName, _InferenceValue value)
        {
            table[index].PlayerName = playerName;
            table[index].Value      = value;
        }
    }
}
