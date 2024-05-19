using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Assassin : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Assassin Instance { get; } = new BoneInitializerRules_Assassin();

        BoneInitializerRules_Assassin() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("AsssassinBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.ShoulderL:
                    case BoneType.ShoulderR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 0f, 180f);
                        break;
                }
            }

            return bone;
        }
    }
}
