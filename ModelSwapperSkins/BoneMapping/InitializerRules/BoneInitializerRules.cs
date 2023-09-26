using ModelSwapperSkins.Utils;
using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public abstract class BoneInitializerRules
    {
        public abstract bool AppliesTo(BodyIndex bodyIndex);

        protected abstract BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform);

        public BoneInfo GetBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo result = getBoneInfo(modelTransform, potentialBoneTransform);
#if DEBUG
            if (result.Type == BoneType.None)
            {
                if (!potentialBoneTransform.GetComponent<RoR2.HurtBox>())
                {
                    Log.Debug($"Unhandled bone in {modelTransform.name}: {TransformUtils.GetObjectPath(potentialBoneTransform, modelTransform)}");
                }
            }
#endif
            return result;
        }
    }
}
