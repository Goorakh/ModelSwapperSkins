using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_ClayGrenadier : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_ClayGrenadier Instance = new BoneInitializerRules_ClayGrenadier();

        protected BoneInitializerRules_ClayGrenadier() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ClayGrenadierBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Head:
                        bone.PositionOffset += new Vector3(0f, -0.05f, -0.2f);
                        bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                        break;
                    case BoneType.LegUpperL:
                    case BoneType.LegUpperR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.ArmUpperL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.HandL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.ArmUpperR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.HandR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                }

                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                case "WholeBody":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, -0.3f, 0f)
                    };
                case "torso":
                    return new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, -0.3f, 0f)
                    };
                default:
                    return bone;
            }
        }
    }
}
