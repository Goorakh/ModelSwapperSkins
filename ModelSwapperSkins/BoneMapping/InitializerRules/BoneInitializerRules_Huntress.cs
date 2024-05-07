using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Huntress : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Huntress Instance { get; } = new BoneInitializerRules_Huntress();

        BoneInitializerRules_Huntress() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("HuntressBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Head:
                        bone.PositionOffset += new Vector3(0f, 0.05f, 0f);
                        break;
                }
            }

            return bone;
        }
    }
}
