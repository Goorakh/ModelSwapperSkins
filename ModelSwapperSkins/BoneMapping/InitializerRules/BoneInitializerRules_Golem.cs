using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Golem : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Golem Instance { get; } = new BoneInitializerRules_Golem();

        BoneInitializerRules_Golem() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("GolemBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Stomach:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.LegUpperL:
                case BoneType.LegLowerL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.LegUpperR:
                case BoneType.LegLowerR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
            }

            return bone;
        }
    }
}
