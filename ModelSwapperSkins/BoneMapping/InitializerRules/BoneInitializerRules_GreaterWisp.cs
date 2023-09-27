using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_GreaterWisp : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_GreaterWisp Instance = new BoneInitializerRules_GreaterWisp();

        protected BoneInitializerRules_GreaterWisp() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("GreaterWispBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                case "Mask":
                    return new BoneInfo(BoneType.Head);
                default:
                    return bone;
            }
        }
    }
}
