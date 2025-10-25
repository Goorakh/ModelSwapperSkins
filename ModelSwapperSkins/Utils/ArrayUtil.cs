using System.Collections.Generic;

namespace ModelSwapperSkins.Utils
{
    public static class ArrayUtil
    {
        public static void Append<T>(ref T[] array, IEnumerable<T> other)
        {
            if (other is null)
                return;

            array = [.. array, .. other];
        }

        public static void Append<T>(ref T[] array, ICollection<T> other)
        {
            if (other == null || other.Count == 0)
                return;

            if (array == null || array.Length == 0)
            {
                array = [.. other];
                return;
            }

            array = [.. array, .. other];
        }

        public static void Append<T>(ref T[] array, T[] other)
        {
            if (other == null || other.Length == 0)
                return;

            if (array == null || array.Length == 0)
            {
                array = other;
                return;
            }

            array = [.. array, .. other];
        }
    }
}
