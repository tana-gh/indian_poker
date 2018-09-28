using System.Collections.Generic;

namespace IndianPoker.Lib
{
    internal class _Player
    {
        public string Name { get; }
        
        private _Inference _Inference { get; }

        public _Player(string name, IEnumerable<_VisibleCard> visibleCards, IEnumerable<_Card> allCards)
        {
            Name = name;
            
            _Inference = new _Inference(name, visibleCards, allCards);
        }

        public void ReceiveAnswer(PlayerAnswer answer)
        {
            _Inference.Update(answer);
        }

        public PlayerAnswer SendAnswer()
        {
            return _Inference.Answer;
        }
    }
}
