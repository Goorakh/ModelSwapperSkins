using BepInEx;
using BepInEx.Bootstrap;
using System.Reflection;

namespace ModelSwapperSkins.ModCompatibility
{
    static class ScrollableLobbyUI
    {
        public const string MOD_GUID = "com.KingEnderBrine.ScrollableLobbyUI";

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
    }
}
