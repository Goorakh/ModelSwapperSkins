using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Gravekeeper : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Gravekeeper Instance = new BoneInitializerRules_Gravekeeper();

        protected BoneInitializerRules_Gravekeeper() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("GravekeeperBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Stomach:
                    case BoneType.Chest:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.HandL:
                    case BoneType.HandR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.FootL:
                    case BoneType.FootR:
                        bone.RotationOffset *= Quaternion.Euler(300f, 0f, 0f);
                        break;
                }

                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                default:
                    return bone;
            }
        }
    }
}
