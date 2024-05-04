using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Shopkeeper : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Shopkeeper Instance { get; } = new BoneInitializerRules_Shopkeeper();

        BoneInitializerRules_Shopkeeper() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ShopkeeperBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            switch (bone.Type)
            {
                case BoneType.Chest:
                    bone.RotationOffset *= Quaternion.Euler(90f, 0f, 0f);
                    break;
                case BoneType.Head:
                    bone.RotationOffset *= Quaternion.Euler(-90f, 180f, 0f);
                    break;
                case BoneType.Jaw:
                    bone.RotationOffset *= Quaternion.Euler(-45f, 0f, 0f);
                    break;
            }

            return bone;
        }
    }
}
