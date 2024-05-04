using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_LunarExploder : BoneInitializerRules
    {
        public static BoneInitializerRules_LunarExploder Instance { get; } = new BoneInitializerRules_LunarExploder();

        BoneInitializerRules_LunarExploder() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("LunarExploderBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "ROOT":
                    return new BoneInfo(BoneType.Root);
                case "stoneBase":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f)
                    };
                case "frontThigh.l":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "frontFoot.l":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "frontThigh.r":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "frontFoot.r":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "backThigh.l":
                    return new BoneInfo(BoneType.LegUpperL);
                case "backFoot.l":
                    return new BoneInfo(BoneType.LegLowerL);
                case "backThigh.r":
                    return new BoneInfo(BoneType.LegUpperR);
                case "backFoot.r":
                    return new BoneInfo(BoneType.LegLowerR);
                default:
                    return BoneInfo.None;
            }
        }
    }
}
