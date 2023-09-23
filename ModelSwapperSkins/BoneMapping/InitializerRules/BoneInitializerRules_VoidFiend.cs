using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_VoidFiend : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_VoidFiend Instance = new BoneInitializerRules_VoidFiend();

        protected BoneInitializerRules_VoidFiend() : base()
        {
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo boneInfo = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (boneInfo.Type != BoneType.None)
                return boneInfo;

            switch (potentialBoneTransform.name)
            {
                case "Hand":
                    return new BoneInfo(BoneType.HandL);
                case "Index1":
                    return new BoneInfo(BoneType.IndexFinger1L);
                case "Index2":
                    return new BoneInfo(BoneType.IndexFinger2L);
                case "Index3":
                    return new BoneInfo(BoneType.IndexFinger3L);
                case "IndexEnd":
                    return new BoneInfo(BoneType.IndexFinger3L_end);
                case "Middle1":
                    return new BoneInfo(BoneType.MiddleFinger1L);
                case "Middle2":
                    return new BoneInfo(BoneType.MiddleFinger2L);
                case "Middle3":
                    return new BoneInfo(BoneType.MiddleFinger3L);
                case "MiddleEnd":
                    return new BoneInfo(BoneType.MiddleFinger3L_end);
                case "Pinky1":
                    return new BoneInfo(BoneType.PinkyFinger1L);
                case "Pinky2":
                    return new BoneInfo(BoneType.PinkyFinger2L);
                case "PinkyEnd":
                    return new BoneInfo(BoneType.PinkyFinger3L);
                case "PinkyEnd_end":
                    return new BoneInfo(BoneType.PinkyFinger3L_end);
                case "Ring1":
                    return new BoneInfo(BoneType.RingFinger1L);
                case "Ring2":
                    return new BoneInfo(BoneType.RingFinger2L);
                case "Ring3":
                    return new BoneInfo(BoneType.RingFinger3L);
                case "RingEnd":
                    return new BoneInfo(BoneType.RingFinger3L_end);
                case "Thumb1":
                    return new BoneInfo(BoneType.Thumb1L);
                case "Thumb2":
                    return new BoneInfo(BoneType.Thumb2L);
                case "ThumbEnd":
                    return new BoneInfo(BoneType.Thumb2L_end);
                default:
                    return BoneInfo.None;
            }
        }
    }
}
