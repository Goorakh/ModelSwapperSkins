using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_DroneTech : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_DroneTech Instance { get; } = new BoneInitializerRules_DroneTech();

        BoneInitializerRules_DroneTech()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("DroneTechBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "arm_robot_R_shldr":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 90f)
                    };
                case "arm_robot_R_elbow":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(270f, 270f, 0f)
                    };
                case "claw_spin_M":
                    return new BoneInfo(BoneType.HandR)
                    {
                        PositionOffset = new Vector3(0f, 0f, -0.1f),
                        RotationOffset = Quaternion.Euler(270f, 180f, 0f)
                    };
                default:
                    BoneInfo boneInfo = base.getBoneInfo(modelTransform, potentialBoneTransform);

                    switch (boneInfo.Type)
                    {
                        case BoneType.Stomach:
                            boneInfo.RotationOffset *= Quaternion.Euler(90f, 270f, 0f);
                            break;
                        case BoneType.Chest:
                            boneInfo.PositionOffset += new Vector3(0.1f, 0f, 0f);
                            boneInfo.RotationOffset *= Quaternion.Euler(90f, 270f, 0f);
                            break;
                        case BoneType.Neck1:
                            boneInfo.RotationOffset *= Quaternion.Euler(270f, 90f, 0f);
                            break;
                        case BoneType.Head:
                            boneInfo.RotationOffset *= Quaternion.Euler(90f, 270f, 0f);
                            break;
                        case BoneType.Pelvis:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 180f);
                            break;
                        case BoneType.LegUpperL:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 270f);
                            break;
                        case BoneType.LegLowerL:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 0f, 90f);
                            break;
                        case BoneType.FootL:
                            boneInfo.RotationOffset *= Quaternion.Euler(45f, 90f, 180f);
                            break;
                        case BoneType.Toe1L:
                            boneInfo.RotationOffset *= Quaternion.Euler(270f, 90f, 0f);
                            break;
                        case BoneType.LegUpperR:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 0f, 270f);
                            break;
                        case BoneType.LegLowerR:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 90f);
                            break;
                        case BoneType.FootR:
                            boneInfo.RotationOffset *= Quaternion.Euler(315f, 270f, 0f);
                            break;
                        case BoneType.Toe1R:
                            boneInfo.RotationOffset *= Quaternion.Euler(90f, 90f, 0f);
                            break;
                        case BoneType.ArmUpperL:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 0f, 90f);
                            break;
                        case BoneType.ArmLowerL:
                            boneInfo.RotationOffset *= Quaternion.Euler(90f, 270f, 0f);
                            break;
                        case BoneType.HandL:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 0f, 90f);
                            break;
                        case BoneType.ArmUpperR:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 90f);
                            break;
                        case BoneType.ArmLowerR:
                            boneInfo.RotationOffset *= Quaternion.Euler(270f, 270f, 0f);
                            break;
                        case BoneType.HandR:
                            boneInfo.RotationOffset *= Quaternion.Euler(90f, 90f, 0f);
                            break;
                    }

                    return boneInfo;
            }
        }
    }
}
