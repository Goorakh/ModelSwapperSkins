using HarmonyLib;
using HG;
using ModelSwapperSkins.Utils;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.Patches
{
    static class AdditionalRendererInfoApplyPatch
    {
        [SystemInitializer]
        static void Init()
        {
            IL.RoR2.CharacterModel.UpdateRendererMaterials += CharacterModel_UpdateRendererMaterials;
        }

        static void CharacterModel_UpdateRendererMaterials(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(MoveType.Before,
                              x => x.MatchCallOrCallvirt(AccessTools.DeclaredPropertySetter(typeof(Renderer), nameof(Renderer.sharedMaterials)))))
            {
                VariableDefinition materialsVar = new VariableDefinition(il.Import(typeof(Material)).MakeArrayType());
                il.Method.Body.Variables.Add(materialsVar);

                c.Emit(OpCodes.Stloc, materialsVar);
                c.Emit(OpCodes.Dup); // Dup renderer instance
                c.Emit(OpCodes.Ldloc, materialsVar);

                c.EmitDelegate((Renderer renderer, Material[] materials) =>
                {
                    if (renderer.TryGetComponent(out AdditionalRendererInfoProvider rendererInfoProvider))
                    {
                        AdditionalRendererInfo rendererInfo = rendererInfoProvider.AdditionalRendererInfo;
                        ArrayUtil.Append(ref materials, rendererInfo.Materials);
                    }

                    return materials;
                });
            }
            else
            {
                Log.Error("Failed to find patch location");
            }
        }
    }
}
