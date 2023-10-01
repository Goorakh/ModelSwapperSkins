using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Toolbot : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Toolbot Instance = new BoneInitializerRules_Toolbot();

        protected BoneInitializerRules_Toolbot() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ToolbotBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.RotationOffset *= Quaternion.Euler(-45f, 180f, 0f);
                    break;
                case BoneType.LegUpperL:
                    bone.RotationOffset *= Quaternion.Euler(0f, -90f, 0f);
                    break;
                case BoneType.LegUpperR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.ArmUpperL:
                case BoneType.ArmLowerL:
                case BoneType.ArmUpperR:
                case BoneType.ArmLowerR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.Pelvis:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.None:
                    switch (potentialBoneTransform.name)
                    {
                        case "mainWheel.l":
                            return new BoneInfo(BoneType.FootL);
                        case "mainWheel.r":
                            return new BoneInfo(BoneType.FootR);
                        case "neckSpin":
                            return new BoneInfo(BoneType.Neck1);
                        case "neckJoint.1":
                            return new BoneInfo(BoneType.Neck2);
                        case "neckJoint.2":
                            return new BoneInfo(BoneType.Neck3);
                        case "toolbase":
                            return new BoneInfo(BoneType.HandL)
                            {
                                MatchFlags = BoneMatchFlags.AllowMatchTo
                            };
                    }

                    break;
            }

            return bone;
        }
    }
}
