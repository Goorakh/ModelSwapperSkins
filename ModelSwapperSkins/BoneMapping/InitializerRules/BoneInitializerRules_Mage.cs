using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Mage : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Mage Instance = new BoneInitializerRules_Mage();

        protected BoneInitializerRules_Mage() : base()
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
                }
            }

            return bone;
        }
    }
}
