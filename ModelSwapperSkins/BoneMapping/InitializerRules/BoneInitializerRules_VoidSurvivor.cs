using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_VoidSurvivor : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_VoidSurvivor Instance { get; } = new BoneInitializerRules_VoidSurvivor();

        BoneInitializerRules_VoidSurvivor() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("VoidSurvivorBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo boneInfo = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (boneInfo.Type == BoneType.None)
            {
                switch (potentialBoneTransform.name)
                {
                    case "Hand":
                        boneInfo = new BoneInfo(BoneType.HandL);
                        break;
                    case "Index1":
                        boneInfo = new BoneInfo(BoneType.IndexFinger1L);
                        break;
                    case "Index2":
                        boneInfo = new BoneInfo(BoneType.IndexFinger2L);
                        break;
                    case "Index3":
                        boneInfo = new BoneInfo(BoneType.IndexFinger3L);
                        break;
                    case "Middle1":
                        boneInfo = new BoneInfo(BoneType.MiddleFinger1L);
                        break;
                    case "Middle2":
                        boneInfo = new BoneInfo(BoneType.MiddleFinger2L);
                        break;
                    case "Middle3":
                        boneInfo = new BoneInfo(BoneType.MiddleFinger3L);
                        break;
                    case "Pinky1":
                        boneInfo = new BoneInfo(BoneType.PinkyFinger1L);
                        break;
                    case "Pinky2":
                        boneInfo = new BoneInfo(BoneType.PinkyFinger2L);
                        break;
                    case "PinkyEnd":
                        boneInfo = new BoneInfo(BoneType.PinkyFinger3L);
                        break;
                    case "Ring1":
                        boneInfo = new BoneInfo(BoneType.RingFinger1L);
                        break;
                    case "Ring2":
                        boneInfo = new BoneInfo(BoneType.RingFinger2L);
                        break;
                    case "Ring3":
                        boneInfo = new BoneInfo(BoneType.RingFinger3L);
                        break;
                    case "Thumb1":
                        boneInfo = new BoneInfo(BoneType.Thumb1L);
                        break;
                    case "Thumb2":
                        boneInfo = new BoneInfo(BoneType.Thumb2L);
                        break;
                    case "ThumbEnd":
                        boneInfo = new BoneInfo(BoneType.Thumb2L_end);
                        break;
                }
            }

            switch (boneInfo.Type)
            {
                case BoneType.Pelvis:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;

                case BoneType.Neck1:
                    boneInfo.PositionOffset += new Vector3(0f, 0.15f, -0.05f);
                    boneInfo.RotationOffset *= Quaternion.Euler(90f, 180f, 0f);
                    break;

                case BoneType.LegUpperL:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.LegLowerL:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.FootL:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.Toe1L:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;

                case BoneType.LegUpperR:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.LegLowerR:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.FootR:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1R:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;

                case BoneType.ShoulderL:
                    boneInfo.PositionOffset += new Vector3(-0.15f, 0f, 0f);
                    boneInfo.RotationOffset *= Quaternion.Euler(45f, 90f, 0f);
                    break;
                case BoneType.ArmLowerL:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.HandL:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;

                case BoneType.ShoulderR:
                    boneInfo.PositionOffset += new Vector3(0.15f, 0f, 0f);
                    boneInfo.RotationOffset *= Quaternion.Euler(45f, 270f, 0f);
                    break;
                case BoneType.ArmUpperR:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmLowerR:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
            }

            return boneInfo;
        }
    }
}
