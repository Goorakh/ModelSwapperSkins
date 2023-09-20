using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    public class SyncTransform : MonoBehaviour
    {
        public Transform Target;

        void LateUpdate ()
        {
            if (!Target)
                return;

            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}
