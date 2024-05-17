using R2API.Utils;
using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_BeetleGuard : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_BeetleGuard Instance { get; } = new BoneInitializerRules_BeetleGuard();

        BoneInitializerRules_BeetleGuard() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("BeetleGuardBody") || bodyIndex == BodyCatalog.FindBodyIndex("BeetleGuardAllyBody") || bodyIndex == BodyCatalog.FindBodyIndex("PaladinBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "base":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
            }

            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.Chest:
                    bone.Scale *= 1.35f;
                    break;
                case BoneType.Pelvis:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmLowerL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    bone.Scale *= 2.5f;
                    break;
                case BoneType.ArmLowerR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    bone.Scale *= 2.5f;
                    break;
                case BoneType.HandL:
                    bone.Scale *= 2f;
                    break;
                case BoneType.HandR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    bone.Scale *= 2f;
                    break;
                case BoneType.LegUpperL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    bone.Scale *= 0.8f;
                    break;
                case BoneType.LegUpperR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    bone.Scale *= 0.8f;
                    break;
                case BoneType.LegLowerR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
            }

            return bone;
        }
    }
}
