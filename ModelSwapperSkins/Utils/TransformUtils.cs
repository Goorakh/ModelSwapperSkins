using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ModelSwapperSkins.Utils
{
    public static class TransformUtils
    {
        public static string GetObjectPath(Transform obj, Transform root)
        {
            if (obj == root)
                return obj.name;

            StringBuilder sb = HG.StringBuilderPool.RentStringBuilder();
            try
            {

                do
                {
                    if (sb.Length == 0)
                    {
                        sb.Append(obj.name);
                    }
                    else
                    {
                        sb.Insert(0, obj.name + "/");
                    }

                    obj = obj.parent;
                } while (obj != null && obj != root);

                return sb.ToString();
            }
            finally
            {
                sb = HG.StringBuilderPool.ReturnStringBuilder(sb);
            }
        }

        public static IEnumerable<Transform> GetAllChildrenRecursive(Transform root)
        {
            yield return root;

            for (int i = 0; i < root.childCount; i++)
            {
                foreach (Transform child in GetAllChildrenRecursive(root.GetChild(i)))
                {
                    yield return child;
                }
            }
        }
    }
}
