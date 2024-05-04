using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Clay : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Clay Instance { get; } = new BoneInitializerRules_Clay();

        BoneInitializerRules_Clay() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ClayBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, -0.15f, 0f);
                    break;
            }

            return bone;
        }
    }
}
