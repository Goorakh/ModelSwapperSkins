using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_MiniMushroom : BoneInitializerRules
    {
        public static BoneInitializerRules_MiniMushroom Instance { get; } = new BoneInitializerRules_MiniMushroom();

        BoneInitializerRules_MiniMushroom() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("MiniMushroomBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "miniMush_TrajectorySHJnt":
                    return new BoneInfo(BoneType.Root);
                case "miniMush_ROOTSHJnt":
                    return new BoneInfo(BoneType.Base)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 180f)
                    };
                case "miniMush_Spine_TopSHJnt":
                    return new BoneInfo(BoneType.Head)
                    {
                        RotationOffset = Quaternion.Euler(90f, 270f, 0f)
                    };
                case "miniMush_Spine_02SHJnt":
                    return new BoneInfo(BoneType.Chest)
                    {
                        RotationOffset = Quaternion.Euler(90f, 270f, 0f)
                    };
                case "miniMush_Spine_01SHJnt":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        RotationOffset = Quaternion.Euler(90f, 270f, 0f)
                    };
                case "miniMush_L_Leg_HipSHJnt":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "miniMush_L_Leg_KneeSHJnt":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "miniMush_L_Leg_AnkleSHJnt":
                    return new BoneInfo(BoneType.FootL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "miniMush_R_Leg_HipSHJnt":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "miniMush_R_Leg_KneeSHJnt":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "miniMush_R_Leg_AnkleSHJnt":
                    return new BoneInfo(BoneType.FootR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                default:
                    return BoneInfo.None;
            }
        }
    }
}
