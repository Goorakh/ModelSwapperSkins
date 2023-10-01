using BepInEx;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Diagnostics;

namespace ModelSwapperSkins
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(R2API.LanguageAPI.PluginGUID)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Gorakh";
        public const string PluginName = "ModelSwapperSkins";
        public const string PluginVersion = "1.0.0";

        void Awake()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Log.Init(Logger);

            SystemInitializerInjector.InjectDependency(typeof(SkinCatalog), typeof(DynamicSkinAdder));

            DynamicSkinAdder.AddSkins += DynamicSkinAdder_AddSkins;

            stopwatch.Stop();
            Log.Info_NoCallerPrefix($"Initialized in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        }

        static void DynamicSkinAdder_AddSkins(SurvivorDef survivor, List<SkinDef> skins)
        {
            SurvivorSkinsInitializer skinsInitializer = new SurvivorSkinsInitializer(survivor);
            skinsInitializer.TryCreateSkins(skins);
        }
    }
}
