using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Seeker : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Seeker Instance { get; } = new BoneInitializerRules_Seeker();

        BoneInitializerRules_Seeker() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("SeekerBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Head:
                        bone.PositionOffset += new Vector3(0f, -0.1f, 0f);
                        break;
                    case BoneType.Pelvis:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 180f);
                        break;
                    case BoneType.ShoulderR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.ArmUpperL:
                    case BoneType.ArmUpperR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.ArmLowerR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.HandL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.LegUpperL:
                    case BoneType.LegUpperR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.LegLowerL:
                    case BoneType.LegLowerR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.FootL:
                    case BoneType.FootR:
                        bone.PositionOffset += new Vector3(-0.05f, 0.05f, 0f);
                        bone.RotationOffset *= Quaternion.Euler(315f, 270f, 0f);
                        break;
                    case BoneType.Toe1L:
                    case BoneType.Toe1R:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    default:
                        break;
                }

                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                case "Spine1":
                    return new BoneInfo(BoneType.Base);
                case "Spine2":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, -0.15f, 0f)
                    };
                case "Spine3":
                    return new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, -0.2f, 0f)
                    };
            }

            return BoneInfo.None;
        }
    }
}
