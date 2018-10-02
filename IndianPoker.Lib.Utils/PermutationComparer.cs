using System.Collections.Generic;
using System.Linq;

namespace IndianPoker.Lib.Utils
{
    // 数列の比較子
    public class PermutationComparer : IComparer<IEnumerable<int>>
    {
        public static PermutationComparer Instance { get; } = new PermutationComparer();

        private PermutationComparer()
        {
        }

        public int Compare(IEnumerable<int> x, IEnumerable<int> y)
        {
            foreach (var (xx, yy) in x.Zip(y, (xx, yy) => (xx, yy)))
            {
                if (xx == yy)
                {
                    continue;
                }

                return xx - yy;
            }

            return 0;
        }
    }
}
