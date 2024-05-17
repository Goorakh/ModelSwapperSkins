using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Titan : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Titan Instance { get; } = new BoneInitializerRules_Titan();

        BoneInitializerRules_Titan() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("TitanBody") || bodyIndex == BodyCatalog.FindBodyIndex("TitanGold");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.ArmLowerL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.ArmUpperR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.ArmLowerR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.HandL:
                case BoneType.HandR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
            }

            return bone;
        }
    }
}
