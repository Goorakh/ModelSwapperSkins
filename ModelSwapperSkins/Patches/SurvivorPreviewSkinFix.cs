using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace ModelSwapperSkins.Patches
{
    static class SurvivorPreviewSkinFix
    {
        [SystemInitializer]
        static void Init()
        {
            IL.RoR2.CharacterSelectSurvivorPreviewDisplayController.RunDefaultResponses += convertLoadoutSkinIndex;
            IL.RoR2.CharacterSelectSurvivorPreviewDisplayController.OnLoadoutChangedGlobal += convertLoadoutSkinIndex;
        }

        static uint tryConvertToBaseSkinIndex(uint skinIndex, BodyIndex bodyIndex)
        {
            SkinDef skin = SkinCatalog.GetBodySkinDef(bodyIndex, (int)skinIndex);
            if (skin is ModelSwappedSkinDef modelSwappedSkin)
            {
                if (modelSwappedSkin.baseSkins != null && modelSwappedSkin.baseSkins.Length > 0)
                {
                    int baseSkinIndex = SkinCatalog.FindLocalSkinIndexForBody(bodyIndex, modelSwappedSkin.baseSkins[0]);
                    if (baseSkinIndex >= 0)
                    {
                        return (uint)baseSkinIndex;
                    }
                }
            }

            return skinIndex;
        }

        static void convertLoadoutSkinIndex(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            while (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt<Loadout.BodyLoadoutManager>(nameof(Loadout.BodyLoadoutManager.GetSkinIndex))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate((uint skinIndex, CharacterSelectSurvivorPreviewDisplayController instance) =>
                {
                    BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(instance.bodyPrefab);
                    if (bodyIndex == BodyIndex.None)
                        return skinIndex;

                    return tryConvertToBaseSkinIndex(skinIndex, bodyIndex);
                });

                c.Index++;
            }
        }
    }
}
