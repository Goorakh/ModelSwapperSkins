using ModelSwapperSkins.Utils;
using RoR2;
using System.Collections.Generic;
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

            if (result.Type == BoneType.None)
            {
                if (!potentialBoneTransform.GetComponent<HurtBox>())
                {
                    Log.Debug($"Unhandled bone in {modelTransform.name}: {TransformUtils.GetObjectPath(potentialBoneTransform, modelTransform)}");
                }
            }

            return result;
        }

        public virtual IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            yield break;
        }
    }
}
