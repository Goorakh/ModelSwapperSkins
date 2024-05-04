using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Brother : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Brother Instance { get; } = new BoneInitializerRules_Brother();

        BoneInitializerRules_Brother() : base()
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
                case BoneType.Head:
                    boneInfo.PositionOffset += new Vector3(0f, -0.25f, 0f);
                    break;
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
