using BepInEx;
using BepInEx.Bootstrap;
using System.Linq;
using System.Reflection;

namespace ModelSwapperSkins.ModCompatibility
{
    static class SkinTuner
    {
        public const string MOD_GUID = "RiskOfResources.SkinTuner";

        public static bool IsActive => Chainloader.PluginInfos.ContainsKey(MOD_GUID);

        public static Assembly Assembly
        {
            get
            {
                if (!Chainloader.PluginInfos.TryGetValue(MOD_GUID, out PluginInfo pluginInfo))
                    return null;

                BaseUnityPlugin pluginInstance = pluginInfo.Instance;
                if (!pluginInstance)
                    return null;

                return pluginInstance.GetType().Assembly;
            }
        }

        public const string SKIN_PANEL_NAME = "SkinPanel";
    }
}
