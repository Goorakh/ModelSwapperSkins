using BepInEx;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Diagnostics;

namespace ModelSwapperSkins
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Gorakh";
        public const string PluginName = "ModelSwapperSkins";
        public const string PluginVersion = "1.5.2";

        void Awake()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Log.Init(Logger);

            LanguageFolderHandler.Register(System.IO.Path.GetDirectoryName(Info.Location));

            SystemInitializerInjector.InjectDependency(typeof(SkinCatalog), typeof(DynamicSkinAdder));

            DynamicSkinAdder.AddSkins += DynamicSkinAdder_AddSkins;

            stopwatch.Stop();
            Log.Info_NoCallerPrefix($"Initialized in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        }

        static void DynamicSkinAdder_AddSkins(CharacterBody bodyPrefab, List<SkinDef> skins)
        {
            BodySkinsInitializer skinsInitializer = new BodySkinsInitializer(bodyPrefab);
            skinsInitializer.TryCreateSkins(skins);
        }
    }
}
