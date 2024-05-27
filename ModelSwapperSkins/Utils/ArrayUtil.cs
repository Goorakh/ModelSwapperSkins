using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelSwapperSkins.Utils
{
    public static class ArrayUtil
    {
        public static void Append<T>(ref T[] array, IEnumerable<T> other)
        {
            if (other is null)
                return;

            Append(ref array, other.ToArray());
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

            int oldLength = array.Length;

            Array.Resize(ref array, oldLength + other.Length);
            Array.Copy(other, 0, array, oldLength, other.Length);
        }
    }
}
