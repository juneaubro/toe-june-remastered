using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

namespace PileOMods
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class PileOModsBase : BaseUnityPlugin
    {
        private const string modGUID = "PileOMods";
        private const string modName = "PileOMods";
        private const string modVersion = "1.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static PileOModsBase Instance;

        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("PileOMods started.");

            // Base
            harmony.PatchAll(typeof(PileOModsBase));
        }
    }
}
