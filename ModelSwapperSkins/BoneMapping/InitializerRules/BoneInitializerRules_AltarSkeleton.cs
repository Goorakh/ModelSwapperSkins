using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_AltarSkeleton : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_AltarSkeleton Instance { get; } = new BoneInitializerRules_AltarSkeleton();

        BoneInitializerRules_AltarSkeleton() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("AltarSkeletonBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, -0.08f, 0f);
                    break;
            }

            return bone;
        }
    }
}
