using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_HermitCrab : BoneInitializerRules
    {
        public static BoneInitializerRules_HermitCrab Instance { get; } = new BoneInitializerRules_HermitCrab();

        BoneInitializerRules_HermitCrab() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("HermitCrabBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "ROOT":
                    return new BoneInfo(BoneType.Root);
                case "Base":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, -0.1f, 0f),
                        RotationOffset = Quaternion.Euler(90f, -40f, 0f)
                    };
                case "leg1.thigh.l":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "leg1.calf.l":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "leg1.foot.l":
                    return new BoneInfo(BoneType.HandL);
                case "leg1.thigh.r":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "leg1.calf.r":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "leg1.foot.r":
                    return new BoneInfo(BoneType.HandR);
                case "leg3.thigh.l":
                    return new BoneInfo(BoneType.LegUpperL);
                case "leg3.calf.l":
                    return new BoneInfo(BoneType.LegLowerL);
                case "leg3.foot.l":
                    return new BoneInfo(BoneType.FootL);
                case "leg3.thigh.r":
                    return new BoneInfo(BoneType.LegUpperR);
                case "leg3.calf.r":
                    return new BoneInfo(BoneType.LegLowerR);
                case "leg3.foot.r":
                    return new BoneInfo(BoneType.FootR);
                default:
                    return BoneInfo.None;
            }
        }
    }
}
