using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Bison : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Bison Instance = new BoneInitializerRules_Bison();

        protected BoneInitializerRules_Bison() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("BisonBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type == BoneType.None)
            {
                switch (potentialBoneTransform.name)
                {
                    case "thigh.b.l":
                        return new BoneInfo(BoneType.LegUpperL);
                    case "calf.b.l":
                        return new BoneInfo(BoneType.LegLowerL)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "foot.b.l":
                        return new BoneInfo(BoneType.FootL);
                    case "toe.b.l":
                        return new BoneInfo(BoneType.Toe1L);
                    case "thigh.b.r":
                        return new BoneInfo(BoneType.LegUpperR);
                    case "calf.b.r":
                        return new BoneInfo(BoneType.LegLowerR)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "foot.b.r":
                        return new BoneInfo(BoneType.FootR);
                    case "toe.b.r":
                        return new BoneInfo(BoneType.Toe1R);
                    case "thigh.f.l":
                        return new BoneInfo(BoneType.ArmUpperL)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "calf.f.l":
                        return new BoneInfo(BoneType.ArmLowerL);
                    case "foot.f.l":
                        return new BoneInfo(BoneType.HandL);
                    case "thigh.f.r":
                        return new BoneInfo(BoneType.ArmUpperR)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "calf.f.r":
                        return new BoneInfo(BoneType.ArmLowerR);
                    case "foot.f.r":
                        return new BoneInfo(BoneType.HandR);
                    default:
                        return bone;
                }
            }

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.RotationOffset = Quaternion.Euler(315f, 180f, 0f);
                    break;
                case BoneType.Pelvis:
                case BoneType.Stomach:
                    bone.RotationOffset = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.Chest:
                    bone.RotationOffset = Quaternion.Euler(0f, 180f, 0f);
                    bone.Scale = new Vector3(2f, 2f, 2f);
                    break;
            }

            return bone;
        }
    }
}
