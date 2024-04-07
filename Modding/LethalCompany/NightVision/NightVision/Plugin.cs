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

            // Base
            harmony.PatchAll(typeof(NightVisionBase));
            harmony.PatchAll(typeof(NightVisionPatch));

            // Player
            harmony.PatchAll(typeof(Player));
            harmony.PatchAll(typeof(GrabDistancePatch));
            harmony.PatchAll(typeof(GodMode));
            harmony.PatchAll(typeof(NoClip));
            harmony.PatchAll(typeof(BoomDead));
            harmony.PatchAll(typeof(TeleportInBuildingEntrance));
            harmony.PatchAll(typeof(TeleportToShip));
            harmony.PatchAll(typeof(TPSnapshot));
            harmony.PatchAll(typeof(RevivePlayers));
            harmony.PatchAll(typeof(ThirdPerson));
            harmony.PatchAll(typeof(XPGain));
            harmony.PatchAll(typeof(StartLever));
            harmony.PatchAll(typeof(WhatAreLocks));
            harmony.PatchAll(typeof(IgnoreTwoHands));
            if(!Chainloader.PluginInfos.ContainsKey("FlipMods.BetterStamina"))
                harmony.PatchAll(typeof(InfiniteStamina)); // don't load if better stamina is installed, causes awake() error
            //harmony.PatchAll(typeof(NoWeight));
            //harmony.PatchAll(typeof(MovementSpeed));

            // Enemy
            harmony.PatchAll(typeof(SpawnEnemy));
            harmony.PatchAll(typeof(FlowermanUnstuck));
            //harmony.PatchAll(typeof(MaskedEnemyUnstuck));

            // Debug
            harmony.PatchAll(typeof(RemoveOriginalDebugs));
        }
    }
}
