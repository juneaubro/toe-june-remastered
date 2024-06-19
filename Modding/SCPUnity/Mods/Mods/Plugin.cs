using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Mods.Patches;

namespace Mods
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ModsBase : BaseUnityPlugin
    {
        private const string modGUID = "JoesMod";
        private const string modName = "JoesMod";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static ModsBase Instance;

        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("JoesMod started.\nJoesMod started.\nJoesMod started.\nJoesMod started.\nJoesMod started.\nJoesMod started.\nJoesMod started.\nJoesMod started.");

            // Base
            harmony.PatchAll(typeof(ModsBase));

            //Debug
            //harmony.PatchAll(typeof(TestMod));
            harmony.PatchAll(typeof(MainMenuPatch));
            harmony.PatchAll(typeof(EnginePatch));
        }
    }
}
