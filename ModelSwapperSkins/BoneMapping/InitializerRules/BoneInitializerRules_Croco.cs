using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Croco : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Croco Instance = new BoneInitializerRules_Croco();

        protected BoneInitializerRules_Croco() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("CrocoBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo boneInfo = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (boneInfo.Type)
            {
                case BoneType.Stomach:
                case BoneType.Chest:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.Head:
                    boneInfo.RotationOffset = Quaternion.Euler(270f, 180f, 0f);
                    break;
                case BoneType.Jaw:
                    boneInfo.RotationOffset *= Quaternion.Euler(-30f, 0f, 0f);
                    break;
                case BoneType.ShoulderL:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmUpperL:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmLowerL:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.HandL:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.FootL:
                case BoneType.FootR:
                    boneInfo.RotationOffset = Quaternion.Euler(290f, 0f, 0f);
                    break;
                case BoneType.ShoulderR:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmUpperR:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmLowerR:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.HandR:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1L:
                case BoneType.Toe1R:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 180f, 0f);
                    break;
            }

            return boneInfo;
        }
    }
}
