using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_FlyingVermin : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_FlyingVermin Instance = new BoneInitializerRules_FlyingVermin();

        protected BoneInitializerRules_FlyingVermin() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("FlyingVerminBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                case "Body":
                    return new BoneInfo(BoneType.Chest);
                case "Wing1.l":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "Wing2.l":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "Wing3.l":
                    return new BoneInfo(BoneType.HandL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Wing1.r":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "Wing2.r":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "Wing3.r":
                    return new BoneInfo(BoneType.HandR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                default:
                    return bone;
            }
        }
    }
}
