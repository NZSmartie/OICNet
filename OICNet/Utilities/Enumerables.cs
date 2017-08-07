using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICNet.Utilities
{
    internal static class Enumerables
    {
        // Thanks to https://stackoverflow.com/a/22165416 for this quick fix
        internal static bool NullRespectingSequenceEqual<T>(
            this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null && second == null)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return false;
            }
            return first.SequenceEqual(second);
        }
    }
}
