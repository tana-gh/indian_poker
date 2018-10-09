using System.Collections.Generic;

namespace IndianPoker.Lib
{
    internal class _Player
    {
        // プレイヤーの名前
        public string Name { get; }
        
        // 推論情報
        private _Inference _Inference { get; }

        // name: プレイヤーの名前
        // visibleCards: 他プレイヤーのカード
        // allCards    : 全カード
        // order       : プレイヤーの順序
        public _Player(string name, IEnumerable<_VisibleCard> visibleCards, IEnumerable<_Card> allCards, _PlayerOrder order)
        {
            Name       = name;
            _Inference = new _Inference(name, visibleCards, allCards, order);
        }

        // 答えを宣言する
        // orderIndex: _PlayerOrder上における現在の順番
        public PlayerAnswer SayAnswer(int orderIndex)
        {
            return _Inference.Infer(orderIndex);
        }
    }
}
