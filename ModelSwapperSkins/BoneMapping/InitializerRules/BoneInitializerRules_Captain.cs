using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Captain : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Captain Instance = new BoneInitializerRules_Captain();

        protected BoneInitializerRules_Captain() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("CaptainBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Toe1L:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1R:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
            }

            return bone;
        }
    }
}
