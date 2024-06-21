using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SCMP.Patches;

namespace SCMP
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        private static Plugin Instance;

        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            mls.LogInfo($"{PluginInfo.PLUGIN_GUID} started loading.");

            // Base
            harmony.PatchAll(typeof(Plugin));

            //Debug
            harmony.PatchAll(typeof(MainMenuPatch));
            harmony.PatchAll(typeof(EnginePatch));
        }
    }
}