using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
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
                if (self is ModelSwappedSkinDef modelSkin)
                {
                    modelSkin.RemoveFrom(modelObject.transform, existingModelObjectTracker.SkinModelObject);
                }

                GameObject.Destroy(existingModelObjectTracker);
            }

            orig(self, modelObject);

            if (self is ModelSwappedSkinDef modelSwappedSkin)
            {
                Transform skinModelTransform = modelSwappedSkin.OnAppliedTo(modelObject.transform);

                SkinModelObjectTracker modelObjectTracker = modelObject.AddComponent<SkinModelObjectTracker>();
                modelObjectTracker.SkinModelObject = skinModelTransform.gameObject;
            }
        }

        class SkinModelObjectTracker : MonoBehaviour
        {
            public GameObject SkinModelObject;

            void OnDestroy()
            {
                if (SkinModelObject)
                {
                    Destroy(SkinModelObject);
                }
            }
        }
    }
}
