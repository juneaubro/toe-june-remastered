using BepInEx.Logging;
using BepInEx;
using HarmonyLib;

namespace RandomMods
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "AcridStartedIt";
        private const string modName = "AcridStartedIt";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin Instance;

        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo($"{modGUID} started loading.");

            // Base
            harmony.PatchAll(typeof(Plugin));


        }
    }
}

