using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Gravekeeper : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Gravekeeper Instance { get; } = new BoneInitializerRules_Gravekeeper();

        BoneInitializerRules_Gravekeeper() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("GravekeeperBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Stomach:
                    case BoneType.Chest:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.ShoulderL:
                    case BoneType.ArmUpperL:
                    case BoneType.ShoulderR:
                    case BoneType.ArmUpperR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.ArmLowerL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.HandL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.ArmLowerR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.HandR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.FootL:
                    case BoneType.FootR:
                        bone.RotationOffset *= Quaternion.Euler(300f, 0f, 0f);
                        break;
                    case BoneType.Toe1L:
                    case BoneType.Toe1R:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                }

                switch (bone.Type)
                {
                    case BoneType.LegUpperL:
                    case BoneType.LegUpperR:
                        bone.Scale *= 0.5f;
                        break;
                    case BoneType.LegLowerL:
                    case BoneType.LegLowerR:
                        bone.Scale *= 0.4f;
                        break;
                    case BoneType.FootL:
                    case BoneType.FootR:
                    case BoneType.Toe1L:
                    case BoneType.Toe1R:
                        bone.Scale *= 0.5f;
                        break;
                    case BoneType.ShoulderL:
                    case BoneType.ArmUpperL:
                    case BoneType.ArmLowerL:
                    case BoneType.HandL:
                    case BoneType.IndexFinger1L:
                    case BoneType.IndexFinger2L:
                    case BoneType.IndexFinger3L:
                    case BoneType.MiddleFinger1L:
                    case BoneType.MiddleFinger2L:
                    case BoneType.MiddleFinger3L:
                    case BoneType.RingFinger1L:
                    case BoneType.RingFinger2L:
                    case BoneType.RingFinger3L:
                    case BoneType.Thumb1L:
                    case BoneType.Thumb2L:
                    case BoneType.ShoulderR:
                    case BoneType.ArmUpperR:
                    case BoneType.ArmLowerR:
                    case BoneType.HandR:
                    case BoneType.IndexFinger1R:
                    case BoneType.IndexFinger2R:
                    case BoneType.IndexFinger3R:
                    case BoneType.MiddleFinger1R:
                    case BoneType.MiddleFinger2R:
                    case BoneType.MiddleFinger3R:
                    case BoneType.RingFinger1R:
                    case BoneType.RingFinger2R:
                    case BoneType.RingFinger3R:
                    case BoneType.Thumb1R:
                    case BoneType.Thumb2R:
                        bone.Scale *= 0.8f;
                        break;
                }

                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                default:
                    return bone;
            }
        }
    }
}
