using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_LemurianBruiser : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_LemurianBruiser Instance = new BoneInitializerRules_LemurianBruiser();

        protected BoneInitializerRules_LemurianBruiser() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("LemurianBruiserBody");
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
                    case BoneType.Head:
                        bone.RotationOffset *= Quaternion.Euler(-55f, 180f, 0f);
                        break;
                    case BoneType.FootL:
                    case BoneType.FootR:
                        bone.PositionOffset += new Vector3(0f, 0f, 0.1f);
                        bone.RotationOffset *= Quaternion.Euler(-90f, 0f, 0f);
                        break;
                    case BoneType.ShoulderL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.ArmUpperL:
                    case BoneType.ArmLowerL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.HandL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.ShoulderR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.ArmUpperR:
                    case BoneType.ArmLowerR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.HandR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                }

                return bone;
            }

            return bone;
        }
    }
}
