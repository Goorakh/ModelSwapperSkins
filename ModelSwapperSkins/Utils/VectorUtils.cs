using UnityEngine;

namespace ModelSwapperSkins.Utils
{
    public static class VectorUtils
    {
        public static Vector3 Divide(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }
    }
}
