using HarmonyLib;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using RoR2;
using System;
using System.Reflection;
using UnityEngine.Events;

namespace ModelSwapperSkins.Patches
{
    static class SkinTunerScrollableLobbyNullRefFix
    {
        [SystemInitializer]
        static void Init()
        {
            if (!ModCompatibility.SkinTuner.IsActive || !ModCompatibility.ScrollableLobbyUI.IsActive)
                return;

            Assembly scrollableLobbyAssembly = ModCompatibility.ScrollableLobbyUI.Assembly;
            if (scrollableLobbyAssembly == null)
            {
                Log.Error("Failed to load ScrollableLobbyUI assembly");
                return;
            }

            const string PATCH_TARGET_CLASS_NAME = "ScrollableLobbyUI.UIHooks";
            Type uiHooksClass = scrollableLobbyAssembly.GetType(PATCH_TARGET_CLASS_NAME, false);
            if (uiHooksClass == null)
            {
                Log.Error($"Failed to find class '{PATCH_TARGET_CLASS_NAME}'");
                return;
            }

            const string PATCH_TARGET_METHOD_NAME = "LoadoutPanelControllerRowFinishSetup";
            MethodInfo rebuildLoadoutPanelsMethod = uiHooksClass.GetMethod(PATCH_TARGET_METHOD_NAME, BindingFlags.NonPublic | BindingFlags.Static);
            if (rebuildLoadoutPanelsMethod == null)
            {
                Log.Error($"Failed to find method '{PATCH_TARGET_METHOD_NAME}' in '{uiHooksClass.FullName}'");
                return;
            }

            using DynamicMethodDefinition dynamicMethod = new DynamicMethodDefinition(rebuildLoadoutPanelsMethod);
            using ILContext il = new ILContext(dynamicMethod.Definition);
            ILCursor c = new ILCursor(il);

            if (c.TryFindNext(out ILCursor[] found,
                              x => x.MatchLdftn(out _),
                              x => x.MatchCallOrCallvirt(SymbolExtensions.GetMethodInfo<UnityEvent>(_ => _.AddListener(default)))))
            {
                new ILHook(((MethodReference)found[0].Next.Operand).ResolveReflection(), ScrollableLobbyUI_FixPanelNullRef);
            }
            else
            {
                Log.Error("Failed to find patch method");
            }
        }

        static void ScrollableLobbyUI_FixPanelNullRef(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchLdfld(out FieldReference field) && field.Name == "redirectConstrained"))
            {
                c.Emit(OpCodes.Dup);
                c.EmitDelegate((UnityEngine.Object obj) => (bool)obj);

                ILLabel afterRetLabel = il.DefineLabel();

                c.Emit(OpCodes.Brtrue, afterRetLabel);

                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ret);

                c.MarkLabel(afterRetLabel);
            }
            else
            {
                Log.Error("Failed to find patch location");
            }
        }
    }
}
