using ModelSwapperSkins.Utils;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public abstract class BoneInitializerRules
    {
        protected abstract BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform);

        public BoneInfo GetBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo result = getBoneInfo(modelTransform, potentialBoneTransform);
#if DEBUG
            if (result.Type == BoneType.None)
            {
                Log.Debug($"Unhandled bone in {modelTransform.name}: {TransformUtils.GetObjectPath(potentialBoneTransform, modelTransform)}");
            }
#endif
            return result;
        }
    }
}
