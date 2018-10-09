using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib
{
    // プレイヤーの順序
    internal class _PlayerOrder
    {
        // 全プレイヤーの名前列
        private IEnumerable<string> _PlayerNames { get; }

        // playerNames: 全プレイヤーの名前列
        public _PlayerOrder(IEnumerable<string> playerNames)
        {
            _PlayerNames = playerNames;
        }

        // return: 順序付けられたプレイヤーの名前列
        public IEnumerable<string> GetPlayerNames()
        {
            for (;;)
            {
                foreach (var name in _PlayerNames)
                {
                    yield return name;
                }
            }
        }
    }
}
