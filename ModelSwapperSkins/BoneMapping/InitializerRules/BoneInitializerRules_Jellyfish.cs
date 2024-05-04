using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Jellyfish : BoneInitializerRules
    {
        public static BoneInitializerRules_Jellyfish Instance { get; } = new BoneInitializerRules_Jellyfish();

        BoneInitializerRules_Jellyfish() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("JellyfishBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "Base":
                    return new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, -0.1f, 0f),
                        RotationOffset = Quaternion.Euler(0f, 0f, 180f)
                    };
                case "JellyArm.l.001":
                    return new BoneInfo(BoneType.ShoulderL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "JellyArm.l.002":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "JellyArm.l.003":
                    return new BoneInfo(BoneType.ArmLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "JellyArm.l.004":
                    return new BoneInfo(BoneType.HandL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "JellyArm.r.001":
                    return new BoneInfo(BoneType.ShoulderR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "JellyArm.r.002":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "JellyArm.r.003":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "JellyArm.r.004":
                    return new BoneInfo(BoneType.HandR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                default:
                    return BoneInfo.None;
            }
        }
    }
}
