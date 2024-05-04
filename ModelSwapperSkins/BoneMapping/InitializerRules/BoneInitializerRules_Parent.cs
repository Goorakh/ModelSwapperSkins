using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Parent : BoneInitializerRules
    {
        public static BoneInitializerRules_Parent Instance { get; } = new BoneInitializerRules_Parent();

        BoneInitializerRules_Parent() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ParentBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "Par_TrajectorySHJnt":
                    return new BoneInfo(BoneType.Root);
                case "Par_ROOTSHJnt":
                    return new BoneInfo(BoneType.Pelvis)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "Par_L_Leg_HipSHJnt":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Par_L_Leg_KneeSHJnt":
                    return new BoneInfo(BoneType.LegLowerL);
                case "Par_L_Leg_AnkleSHJnt":
                    return new BoneInfo(BoneType.FootL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "Par_L_Leg_ToeSHJnt":
                    return new BoneInfo(BoneType.Toe1L);
                case "Par_R_Leg_HipSHJnt":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Par_R_Leg_KneeSHJnt":
                    return new BoneInfo(BoneType.LegLowerR);
                case "Par_R_Leg_AnkleSHJnt":
                    return new BoneInfo(BoneType.FootR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "Par_R_Leg_ToeSHJnt":
                    return new BoneInfo(BoneType.Toe1R);
                case "Par_Spine_01SHJnt":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "Par_Spine_02SHJnt":
                    return new BoneInfo(BoneType.Chest)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "Par_Spine_TopSHJnt":
                    return new BoneInfo(BoneType.Head)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "Par_L_Arm_ClavicleSHJnt":
                    return new BoneInfo(BoneType.ShoulderL);
                case "Par_L_Arm_ShoulderSHJnt":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "Par_L_Arm_ElbowSHJnt":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "Par_L_Arm_WristSHJnt":
                    return new BoneInfo(BoneType.HandL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Par_L_Finger_01_01SHJnt":
                    return new BoneInfo(BoneType.IndexFinger1L);
                case "Par_L_Finger_01_02SHJnt":
                    return new BoneInfo(BoneType.IndexFinger2L);
                case "Par_L_Finger_01_03SHJnt":
                    return new BoneInfo(BoneType.IndexFinger3L);
                case "Par_L_Finger_02_01SHJnt":
                    return new BoneInfo(BoneType.PinkyFinger1L);
                case "Par_L_Finger_02_02SHJnt":
                    return new BoneInfo(BoneType.PinkyFinger2L);
                case "Par_L_Finger_02_03SHJnt":
                    return new BoneInfo(BoneType.PinkyFinger3L);
                case "Par_L_Thumb_01_01SHJnt":
                    return new BoneInfo(BoneType.Thumb1L);
                case "Par_L_Thumb_01_02SHJnt":
                    return new BoneInfo(BoneType.Thumb2L);
                case "Par_R_Arm_ClavicleSHJnt":
                    return new BoneInfo(BoneType.ShoulderR);
                case "Par_R_Arm_ShoulderSHJnt":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "Par_R_Arm_ElbowSHJnt":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "Par_R_Arm_WristSHJnt":
                    return new BoneInfo(BoneType.HandR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Par_R_Finger_01_01SHJnt":
                    return new BoneInfo(BoneType.IndexFinger1R);
                case "Par_R_Finger_01_02SHJnt":
                    return new BoneInfo(BoneType.IndexFinger2R);
                case "Par_R_Finger_01_03SHJnt":
                    return new BoneInfo(BoneType.IndexFinger3R);
                case "Par_R_Finger_02_01SHJnt":
                    return new BoneInfo(BoneType.PinkyFinger1R);
                case "Par_R_Finger_02_02SHJnt":
                    return new BoneInfo(BoneType.PinkyFinger2R);
                case "Par_R_Finger_02_03SHJnt":
                    return new BoneInfo(BoneType.PinkyFinger3R);
                case "Par_R_Thumb_01_01SHJnt":
                    return new BoneInfo(BoneType.Thumb1R);
                case "Par_R_Thumb_01_02SHJnt":
                    return new BoneInfo(BoneType.Thumb2R);
                default:
                    return BoneInfo.None;
            }
        }
    }
}
