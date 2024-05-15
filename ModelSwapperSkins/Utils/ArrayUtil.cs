using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelSwapperSkins.Utils
{
    public static class ArrayUtil
    {
        public static void Append<T>(ref T[] array, IEnumerable<T> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            T[] appendArray = collection.ToArray();
            if (array == null || array.Length == 0)
            {
                array = appendArray;
                return;
            }

            if (appendArray.Length > 0)
            {
                int oldLength = array.Length;

                Array.Resize(ref array, oldLength + appendArray.Length);
                Array.Copy(appendArray, 0, array, oldLength, appendArray.Length);
            }
        }
    }
}
