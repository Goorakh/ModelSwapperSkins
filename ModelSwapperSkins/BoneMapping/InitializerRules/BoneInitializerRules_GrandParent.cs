using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_GrandParent : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_GrandParent Instance { get; } = new BoneInitializerRules_GrandParent();

        BoneInitializerRules_GrandParent() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("GrandParentBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Pelvis:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.ArmUpperL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.ArmUpperR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.ArmLowerL:
                    case BoneType.ArmLowerR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                }

                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                case "head.1":
                    return new BoneInfo(BoneType.Head);
                case "finger.1.1.l":
                    return new BoneInfo(BoneType.IndexFinger1L);
                case "finger.1.2.l":
                    return new BoneInfo(BoneType.IndexFinger2L);
                case "finger.1.3.l":
                    return new BoneInfo(BoneType.IndexFinger3L);
                case "finger.2.1.l":
                    return new BoneInfo(BoneType.PinkyFinger1L);
                case "finger.2.2.l":
                    return new BoneInfo(BoneType.PinkyFinger2L);
                case "finger.2.3.l":
                    return new BoneInfo(BoneType.PinkyFinger3L);
                case "finger.1.1.r":
                    return new BoneInfo(BoneType.IndexFinger1R);
                case "finger.1.2.r":
                    return new BoneInfo(BoneType.IndexFinger2R);
                case "finger.1.3.r":
                    return new BoneInfo(BoneType.IndexFinger3R);
                case "finger.2.1.r":
                    return new BoneInfo(BoneType.PinkyFinger1R);
                case "finger.2.2.r":
                    return new BoneInfo(BoneType.PinkyFinger2R);
                case "finger.2.3.r":
                    return new BoneInfo(BoneType.PinkyFinger3R);
                default:
                    return bone;
            }
        }
    }
}
