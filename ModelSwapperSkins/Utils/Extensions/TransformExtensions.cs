using UnityEngine;

namespace ModelSwapperSkins.Utils.Extensions
{
    public static class TransformExtensions
    {
        public static Transform FindChildRecursive(this Transform transform, string childName)
        {
            if (transform.name == childName)
                return transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i).FindChildRecursive(childName);
                if (child)
                    return child;
            }

            return null;
        }
    }
}
