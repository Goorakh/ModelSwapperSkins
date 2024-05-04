using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Heretic : BoneInitializerRules
    {
        public static BoneInitializerRules_Heretic Instance { get; } = new BoneInitializerRules_Heretic();

        BoneInitializerRules_Heretic() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("HereticBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "Master":
                    return new BoneInfo(BoneType.Root);
                case "Main":
                    return new BoneInfo(BoneType.Base);
                case "Root_M":
                    return new BoneInfo(BoneType.Pelvis)
                    {
                        RotationOffset = Quaternion.Euler(90f, 90f, 0f)
                    };
                case "Spine1_M":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        RotationOffset = Quaternion.Euler(270f, 90f, 0f)
                    };
                case "Chest_M":
                    return new BoneInfo(BoneType.Chest)
                    {
                        RotationOffset = Quaternion.Euler(270f, 90f, 0f)
                    };
                case "Head_M":
                    return new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, -0.25f, 0f),
                        RotationOffset = Quaternion.Euler(270f, 90f, 0f)
                    };
                case "Jaw_M":
                    return new BoneInfo(BoneType.Jaw)
                    {
                        RotationOffset = Quaternion.Euler(45f, 270f, 0f)
                    };
                case "Hip_L":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        RotationOffset = Quaternion.Euler(90f, 90f, 0f)
                    };
                case "Knee_L":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(270f, 270f, 0f)
                    };
                case "Ankle_L":
                    return new BoneInfo(BoneType.FootL)
                    {
                        PositionOffset = new Vector3(0f, 0f, 0.05f),
                        RotationOffset = Quaternion.Euler(0f, 90f, 180f)
                    };
                case "Hip_R":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        RotationOffset = Quaternion.Euler(270f, 90f, 0f)
                    };
                case "Knee_R":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(90f, 270f, 0f)
                    };
                case "Ankle_R":
                    return new BoneInfo(BoneType.FootR)
                    {
                        PositionOffset = new Vector3(0f, 0f, 0.05f),
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "Scapula_L":
                    return new BoneInfo(BoneType.ShoulderL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 270f)
                    };
                case "Shoulder_L":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 270f)
                    };
                case "Elbow_L":
                    return new BoneInfo(BoneType.ArmLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 270f)
                    };
                case "Scapula_R":
                    return new BoneInfo(BoneType.ShoulderR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 270f)
                    };
                case "Shoulder_R":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 270f)
                    };
                case "Elbow_R":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 270f)
                    };
                default:
                    return BoneInfo.None;
            }
        }
    }
}
