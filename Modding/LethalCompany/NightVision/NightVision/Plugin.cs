using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using NightVision.Patches;

namespace NightVision
{
    [BepInPlugin(modGUID,modName,modVersion)]
    public class NightVisionBase : BaseUnityPlugin
    {
        private const string modGUID = "GubMod";
        private const string modName = "GubMod";
        private const string  modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static NightVisionBase Instance;

        internal static ManualLogSource mls;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            mls=BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("GUB MOD STARTEDEDEDEDEDEDED.");

            harmony.PatchAll(typeof(NightVisionBase));
            harmony.PatchAll(typeof(NightVisionPatch));
            harmony.PatchAll(typeof(GrabDistancePatch));
            harmony.PatchAll(typeof(GodMode));
            harmony.PatchAll(typeof(RemoveOriginalDebugs));
            harmony.PatchAll(typeof(NoClip));
            harmony.PatchAll(typeof(FlowermanUnstuck));
            harmony.PatchAll(typeof(SpawnFlowerMan));
            harmony.PatchAll(typeof(TeleportInBuildingEntrance));
            harmony.PatchAll(typeof(TeleportToShip));
            harmony.PatchAll(typeof(RevivePlayers));
            harmony.PatchAll(typeof(IgnoreTwoHands));
            harmony.PatchAll(typeof(ThirdPerson));
            if(!Chainloader.PluginInfos.ContainsKey("FlipMods.BetterStamina"))
                harmony.PatchAll(typeof(InfiniteStamina)); // don't load if better stamina is installed, causes awake() error
            //harmony.PatchAll(typeof(NoWeight));
            //harmony.PatchAll(typeof(MovementSpeed));
            harmony.PatchAll(typeof(XPGain));
        }
    }
}
