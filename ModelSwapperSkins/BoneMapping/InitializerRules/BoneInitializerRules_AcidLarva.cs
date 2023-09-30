using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_AcidLarva : BoneInitializerRules
    {
        public static readonly BoneInitializerRules_AcidLarva Instance = new BoneInitializerRules_AcidLarva();

        protected BoneInitializerRules_AcidLarva()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("AcidLarvaBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "Root":
                    return new BoneInfo(BoneType.Root);
                case "BodyBase":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, -0.2f, 0f),
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f)
                    };
                case "Beak.Lower":
                    return new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, 0f, -0.1f),
                        RotationOffset = Quaternion.Euler(300f, 180f, 0f)
                    };
                case "Beak.Upper":
                    return new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, 0f, -0.1f),
                        RotationOffset = Quaternion.Euler(305f, 180f, 0f)
                    };
                case "FrontLeg_Thigh1.L":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "FrontLeg_Calf.L":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "FrontLeg_Foot.L":
                    return new BoneInfo(BoneType.HandL);
                case "FrontLeg_Thigh1.R":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "FrontLeg_Calf.R":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "FrontLeg_Foot.R":
                    return new BoneInfo(BoneType.HandR);
                case "RearLeg_Thigh1.L":
                    return new BoneInfo(BoneType.LegUpperL);
                case "RearLeg_Calf.L":
                    return new BoneInfo(BoneType.LegLowerL);
                case "RearLeg_Foot.L":
                    return new BoneInfo(BoneType.FootL);
                case "RearLeg_Thigh1.R":
                    return new BoneInfo(BoneType.LegUpperR);
                case "RearLeg_Calf.R":
                    return new BoneInfo(BoneType.LegLowerR);
                case "RearLeg_Foot.R":
                    return new BoneInfo(BoneType.FootR);
                default:
                    return BoneInfo.None;
            }
        }
    }
}
