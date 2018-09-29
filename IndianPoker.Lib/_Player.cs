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
        public _Player(string name, IEnumerable<_VisibleCard> visibleCards, IEnumerable<_Card> allCards)
        {
            Name = name;
            
            _Inference = new _Inference(name, visibleCards, allCards);
        }

        // 他プレイヤーの答えを受け取る
        // answer: 受け取る答え
        public void ReceiveAnswer(PlayerAnswer answer)
        {
            _Inference.Update(answer); // 推論情報の更新
        }

        // 自分の手番において答えを宣言する
        public PlayerAnswer SendAnswer()
        {
            _Inference.Update(new PlayerAnswer(Name, AnswerValue.Unknown)); // 自分の手番で更新が必要
            return _Inference.Answer;
        }
    }
}
