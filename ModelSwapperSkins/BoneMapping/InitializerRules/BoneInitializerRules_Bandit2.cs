using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Bandit2 : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Bandit2 Instance { get; } = new BoneInitializerRules_Bandit2();

        BoneInitializerRules_Bandit2() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("Bandit2Body");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, -0.1f, 0f);
                    break;
            }

            return bone;
        }
    }
}
