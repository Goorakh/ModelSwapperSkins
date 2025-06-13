using System.Collections;
using System.Collections.Generic;

namespace ModelSwapperSkins.Utils
{
    public static class ArrayUtil
    {
        public static void Append<T>(ref T[] array, IEnumerable<T> other)
        {
            if (other is null)
                return;

            if (other is ICollection otherCollection && otherCollection.Count == 0)
                return;

            array = [.. array, .. other];
        }

        public static void Append<T>(ref T[] array, T[] other)
        {
            if (array == null || array.Length == 0)
            {
                array = other;
                return;
            }

            if (other == null || other.Length == 0)
                return;

            array = [.. array, .. other];
        }
    }
}
