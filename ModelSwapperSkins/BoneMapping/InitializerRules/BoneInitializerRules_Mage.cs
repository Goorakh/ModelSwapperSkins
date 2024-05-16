using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Mage : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Mage Instance { get; } = new BoneInitializerRules_Mage();

        BoneInitializerRules_Mage() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("MageBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.ArmLowerR:
                    case BoneType.HandR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.LegUpperL:
                    case BoneType.LegUpperR:
                        bone.PositionOffset += new Vector3(0f, 0f, 0.05f);
                        break;
                }
            }

            return bone;
        }
    }
}
