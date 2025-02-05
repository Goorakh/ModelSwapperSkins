using HG;
using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.Patches
{
    // Fixes nullrefs if non-existing paths are put into gameObjectActivationTemplates or meshReplacementTemplates.
    // rendererInfoTemplates is not included in this patch since they are already null-checked properly
    static class FixMissingSkinPathsPatch
    {
        [SystemInitializer]
        static void Init()
        {
            On.RoR2.SkinDef.RuntimeSkin.Apply += RuntimeSkin_Apply;
        }

        static void RuntimeSkin_Apply(On.RoR2.SkinDef.RuntimeSkin.orig_Apply orig, object self, GameObject modelObject)
        {
            if (self is SkinDef.RuntimeSkin skin)
            {
                Transform modelTransform = modelObject.transform;

                if (skin.gameObjectActivationTemplates != null)
                {
                    for (int i = skin.gameObjectActivationTemplates.Length - 1; i >= 0; i--)
                    {
                        if (!modelTransform.Find(skin.gameObjectActivationTemplates[i].path))
                        {
                            Log.Debug($"Removing invalid gameObjectActivationTemplates path \"{skin.gameObjectActivationTemplates[i].path}\"");

                            ArrayUtils.ArrayRemoveAtAndResize(ref skin.gameObjectActivationTemplates, i);
                        }
                    }
                }

                if (skin.meshReplacementTemplates != null)
                {
                    for (int i = skin.meshReplacementTemplates.Length - 1; i >= 0; i--)
                    {
                        if (!modelTransform.Find(skin.meshReplacementTemplates[i].path))
                        {
                            Log.Debug($"Removing invalid meshReplacementTemplates path \"{skin.meshReplacementTemplates[i].path}\"");

                            ArrayUtils.ArrayRemoveAtAndResize(ref skin.meshReplacementTemplates, i);
                        }
                    }
                }
            }

            orig(self, modelObject);
        }
    }
}
