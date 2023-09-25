using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Assassin : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Assassin Instance = new BoneInitializerRules_Assassin();

        protected BoneInitializerRules_Assassin() : base()
        {
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            // This model is so fucking janky i hate it

            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type == BoneType.None)
            {
                switch (potentialBoneTransform.name)
                {
                    case "spine_1_jnt":
                        return new BoneInfo(BoneType.Base)
                        {
                            RotationOffset = Quaternion.Euler(315f, 90f, 0f),
                            PositionOffset = new Vector3(0f, -0.2f, 0f)
                        };
                    case "spine_2_jnt":
                        return new BoneInfo(BoneType.Stomach)
                        {
                            RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                        };
                    case "mask_jnt":
                        return new BoneInfo(BoneType.Head)
                        {
                            RotationOffset = Quaternion.Euler(0f, 270f, 180f),
                            PositionOffset = new Vector3(0f, 0f, -0.1f),
                            MatchFlags = BoneMatchFlags.MatchToOther
                        };
                    default:
                        return bone;
                }
            }

            switch (bone.Type)
            {
                case BoneType.Head when (bone.MatchFlags & BoneMatchFlags.AllowMatchTo) != 0:
                    bone.RotationOffset = Quaternion.Euler(0f, 90f, 0f);
                    break;

                case BoneType.Pelvis:
                    bone.RotationOffset = Quaternion.Euler(90f, 270f, 0f);
                    break;

                case BoneType.Chest:
                    bone.RotationOffset = Quaternion.Euler(270f, 90f, 0f);
                    break;

                case BoneType.LegUpperL:
                    bone.RotationOffset = Quaternion.Euler(0f, 0f, 270f);
                    break;
                case BoneType.LegLowerL:
                    bone.RotationOffset = Quaternion.Euler(0f, 180f, 90f);
                    break;
                case BoneType.FootL:
                    bone.RotationOffset = Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1L:
                    bone.RotationOffset = Quaternion.Euler(270f, 270f, 0f);
                    break;

                case BoneType.LegUpperR:
                    bone.RotationOffset = Quaternion.Euler(0f, 0f, 90f);
                    break;
                case BoneType.LegLowerR:
                    bone.RotationOffset = Quaternion.Euler(0f, 180f, 270f);
                    break;
                case BoneType.FootR:
                    bone.RotationOffset = Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.Toe1R:
                    bone.RotationOffset = Quaternion.Euler(90f, 270f, 0f);
                    break;

                case BoneType.ArmUpperL:
                    bone.RotationOffset = Quaternion.Euler(0f, 0f, 270f);
                    break;
                case BoneType.ArmLowerL:
                    bone.RotationOffset = Quaternion.Euler(0f, 0f, 270f);
                    break;
                case BoneType.HandL:
                    bone.RotationOffset = Quaternion.Euler(270f, 90f, 0f);
                    break;

                case BoneType.ArmUpperR:
                    bone.RotationOffset = Quaternion.Euler(0f, 0f, 90f);
                    break;
                case BoneType.ArmLowerR:
                    bone.RotationOffset = Quaternion.Euler(0f, 0f, 90f);
                    break;
                case BoneType.HandR:
                    bone.RotationOffset = Quaternion.Euler(90f, 270f, 0f);
                    break;
            }

            return bone;
        }
    }
}
