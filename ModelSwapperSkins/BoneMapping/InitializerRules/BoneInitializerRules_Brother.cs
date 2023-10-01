using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Brother : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Brother Instance = new BoneInitializerRules_Brother();

        protected BoneInitializerRules_Brother() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            switch (BodyCatalog.GetBodyName(bodyIndex))
            {
                case "BrotherBody":
                case "BrotherGlassBody":
                case "BrotherHurtBody":
                    return true;
                default:
                    return false;
            }
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo boneInfo = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (boneInfo.Type)
            {
                case BoneType.Toe1L:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1R:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
            }

            return boneInfo;
        }
    }
}
