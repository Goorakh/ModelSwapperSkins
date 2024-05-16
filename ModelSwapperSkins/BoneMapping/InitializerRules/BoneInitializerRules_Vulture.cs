using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Vulture : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Vulture Instance { get; } = new BoneInitializerRules_Vulture();

        BoneInitializerRules_Vulture() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("VultureBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, -0.2f, 0f);
                    bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                    break;
                case BoneType.Neck1:
                case BoneType.Neck2:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ShoulderL:
                case BoneType.ShoulderR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmUpperL:
                case BoneType.ArmLowerL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.ArmUpperR:
                case BoneType.ArmLowerR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.LegUpperL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.FootL:
                    bone.RotationOffset *= Quaternion.Euler(315f, 0f, 0f);
                    break;
                case BoneType.Toe1L:
                case BoneType.Toe2L:
                case BoneType.Toe3L:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.LegUpperR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.FootR:
                    bone.RotationOffset *= Quaternion.Euler(315f, 0f, 0f);
                    break;
                case BoneType.Toe1R:
                case BoneType.Toe2R:
                case BoneType.Toe3R:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.HandL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.HandR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.Jaw:
                    bone.RotationOffset *= Quaternion.Euler(330f, 0f, 0f);
                    break;
            }

            return bone;
        }
    }
}
