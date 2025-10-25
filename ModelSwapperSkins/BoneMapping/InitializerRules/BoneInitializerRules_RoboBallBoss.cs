using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_RoboBallBoss : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_RoboBallBoss Instance { get; } = new BoneInitializerRules_RoboBallBoss();

        BoneInitializerRules_RoboBallBoss()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("RoboBallBossBody") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("SuperRoboBallBossBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "ROOT":
                    return new BoneInfo(BoneType.Head)
                    {
                        Scale = 2f
                    };

                case "Tentacle1.2.l":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Tentacle1.5.l":
                    return new BoneInfo(BoneType.LegLowerL);
                case "Tentacle1.8.l":
                    return new BoneInfo(BoneType.FootL);

                case "Tentacle1.2.r":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Tentacle1.5.r":
                    return new BoneInfo(BoneType.LegLowerR);
                case "Tentacle1.8.r":
                    return new BoneInfo(BoneType.FootR);

                case "Tentacle2.2.l":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Tentacle2.5.l":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "Tentacle2.8.l":
                    return new BoneInfo(BoneType.HandL);

                case "Tentacle2.2.r":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Tentacle2.5.r":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "Tentacle2.8.r":
                    return new BoneInfo(BoneType.HandR);
            }

            return base.getBoneInfo(modelTransform, potentialBoneTransform);
        }
    }
}
