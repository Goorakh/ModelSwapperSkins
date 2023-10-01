using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelSwapperSkins.Utils
{
    public static class ArrayUtil
    {
        public static void Append<T>(ref T[] array, IEnumerable<T> collection)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            T[] appendedArray = collection.ToArray();
            if (appendedArray.Length > 0)
            {
                int oldLength = array.Length;

                Array.Resize(ref array, oldLength + appendedArray.Length);
                Array.Copy(appendedArray, 0, array, oldLength, appendedArray.Length);
            }
        }
    }
}
