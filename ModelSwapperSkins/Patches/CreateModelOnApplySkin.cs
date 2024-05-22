using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.Patches
{
    static class CreateModelOnApplySkin
    {
        [SystemInitializer]
        static void Init()
        {
            On.RoR2.SkinDef.Apply += SkinDef_Apply;
        }

        static void SkinDef_Apply(On.RoR2.SkinDef.orig_Apply orig, SkinDef self, GameObject modelObject)
        {
            if (modelObject.TryGetComponent(out SkinModelObjectTracker existingModelObjectTracker))
            {
                existingModelObjectTracker.AppliedSkin.RemoveFrom(modelObject.transform, existingModelObjectTracker.SkinModelObject);

                GameObject.Destroy(existingModelObjectTracker);
            }

            orig(self, modelObject);

            if (!modelObject.TryGetComponent(out AppliedSkinTracker appliedSkinTracker))
                appliedSkinTracker = modelObject.AddComponent<AppliedSkinTracker>();

            appliedSkinTracker.AppliedSkin = self;

            if (self is ModelSwappedSkinDef modelSwappedSkin)
            {
                Transform skinModelTransform = modelSwappedSkin.InstantiateModel(modelObject.transform);

                SkinModelObjectTracker modelObjectTracker = modelObject.AddComponent<SkinModelObjectTracker>();
                modelObjectTracker.SkinModelObject = skinModelTransform.gameObject;
                modelObjectTracker.AppliedSkin = modelSwappedSkin;
            }
        }

        class SkinModelObjectTracker : MonoBehaviour
        {
            public ModelSwappedSkinDef AppliedSkin;
            public GameObject SkinModelObject;

            void OnDestroy()
            {
                if (SkinModelObject)
                {
                    Destroy(SkinModelObject);
                }
            }
        }

        public class AppliedSkinTracker : MonoBehaviour
        {
            public SkinDef AppliedSkin;
        }
    }
}
