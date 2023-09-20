using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.Utils
{
    public static class TransformUtils
    {
        public static string GetObjectPath(Transform obj, Transform root)
        {
            if (obj == root)
                return obj.name;

            string path = string.Empty;
            do
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = obj.name;
                }
                else
                {
                    path = obj.name + "/" + path;
                }

                obj = obj.parent;
            } while (obj != null && obj != root);

            return path;
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
