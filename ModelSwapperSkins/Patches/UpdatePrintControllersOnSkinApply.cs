using ModelSwapperSkins.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins.Patches
{
    [SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public", Justification = "Patch")]
    static class UpdatePrintControllersOnSkinApply
    {
        [SystemInitializer]
        static void Init()
        {
            On.RoR2.SkinDef.RuntimeSkin.Apply += RuntimeSkin_Apply;
        }

        static void RuntimeSkin_Apply(On.RoR2.SkinDef.RuntimeSkin.orig_Apply orig, object self, GameObject modelObject)
        {
            orig(self, modelObject);

            if (modelObject && modelObject.TryGetComponent(out CharacterModel characterModel))
            {
                foreach (PrintController printController in modelObject.GetComponents<PrintController>())
                {
                    if (!printController.hasSetupOnce)
                        continue;

                    List<PrintController.RendererMaterialPair> newRendererMaterialPairs = new List<PrintController.RendererMaterialPair>(printController.rendererMaterialPairs.Length);

                    foreach (PrintController.RendererMaterialPair rendererMaterialPair in printController.rendererMaterialPairs)
                    {
                        bool printCutoffEnabled = rendererMaterialPair.material.IsKeywordEnabled(ShaderKeywords.PRINT_CUTOFF);
                        int printOn = rendererMaterialPair.material.GetInt(ShaderIDs._PrintOn);

                        GameObject.Destroy(rendererMaterialPair.material);

                        int rendererIndex = Array.FindIndex(characterModel.baseRendererInfos, r => r.renderer == rendererMaterialPair.renderer);
                        if (rendererIndex == -1)
                            continue;

                        ref CharacterModel.RendererInfo rendererInfo = ref characterModel.baseRendererInfos[rendererIndex];
                        if (!rendererInfo.defaultMaterial || rendererInfo.defaultMaterial.shader != PrintController.printShader)
                            continue;

                        Material material = Material.Instantiate(rendererInfo.defaultMaterial);

                        if (printCutoffEnabled)
                        {
                            material.EnableKeyword(ShaderKeywords.PRINT_CUTOFF);
                        }
                        else
                        {
                            material.DisableKeyword(ShaderKeywords.PRINT_CUTOFF);
                        }

                        material.SetInt(ShaderIDs._PrintOn, printOn);

                        rendererInfo.defaultMaterial = material;

                        newRendererMaterialPairs.Add(new PrintController.RendererMaterialPair(rendererMaterialPair.renderer, material));
                    }

                    printController.rendererMaterialPairs = newRendererMaterialPairs.ToArray();
                }
            }
        }
    }
}
