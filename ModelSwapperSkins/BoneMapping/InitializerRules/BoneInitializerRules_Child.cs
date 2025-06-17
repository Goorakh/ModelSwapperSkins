using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Child : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Child Instance { get; } = new BoneInitializerRules_Child();

        BoneInitializerRules_Child()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ChildBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "spine_01":
                    return new BoneInfo(BoneType.Stomach);

                case "l_clavicle":
                    return new BoneInfo(BoneType.ShoulderL)
                    {
                        RotationOffset = Quaternion.Euler(90f, 270f, 0f)
                    };
                case "l_shoulder":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
                case "l_elbow":
                    return new BoneInfo(BoneType.ArmLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
                case "l_wrist":
                    return new BoneInfo(BoneType.HandL)
                    {
                        RotationOffset = Quaternion.Euler(270f, 90f, 0f)
                    };

                case "r_clavicle":
                    return new BoneInfo(BoneType.ShoulderR)
                    {
                        RotationOffset = Quaternion.Euler(270f, 270f, 0f)
                    };
                case "r_shoulder":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 90f)
                    };
                case "r_elbow":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 90f)
                    };
                case "r_wrist":
                    return new BoneInfo(BoneType.HandR)
                    {
                        RotationOffset = Quaternion.Euler(45f, 45f, 315f)
                    };

                case "l_hip":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 180f)
                    };
                case "l_knee":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 180f)
                    };
                case "l_ankle":
                    return new BoneInfo(BoneType.FootL)
                    {
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f)
                    };
                case "l_ball":
                    return new BoneInfo(BoneType.Toe1L)
                    {
                        RotationOffset = Quaternion.Euler(270f, 180f, 0f)
                    };

                case "r_hip":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "r_knee":
                    return new BoneInfo(BoneType.LegLowerR);
                case "r_ankle":
                    return new BoneInfo(BoneType.FootR)
                    {
                        RotationOffset = Quaternion.Euler(270f, 0f, 0f)
                    };
                case "r_ball":
                    return new BoneInfo(BoneType.Toe1R)
                    {
                        RotationOffset = Quaternion.Euler(90f, 180f, 0f)
                    };
            }

            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Chest:
                    bone.PositionOffset += new Vector3(0f, -0.15f, 0f);
                    break;
                case BoneType.Pelvis:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 180f);
                    break;
                case BoneType.Neck1:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
            }

            return bone;
        }
    }
}
