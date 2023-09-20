using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    public abstract class BoneInitializerRules
    {
        public abstract BoneType? TryResolveBoneType(Transform modelTransform, Transform potentialBoneTransform);
    }
}
