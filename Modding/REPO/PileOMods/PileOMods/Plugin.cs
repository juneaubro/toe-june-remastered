using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PileOMods.Patches;

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

            // Player
            harmony.PatchAll(typeof(GodMode));
            harmony.PatchAll(typeof(KYS));
            harmony.PatchAll(typeof(GrabDistance));
            harmony.PatchAll(typeof(SpawnTP));
            harmony.PatchAll(typeof(Stamina));
            harmony.PatchAll(typeof(NoClip_PlayerController));
            harmony.PatchAll(typeof(NoClip_PlayerAvatar));
            harmony.PatchAll(typeof(EnemyList));
            harmony.PatchAll(typeof(EnemyOnMap));

            // Enemy
            harmony.PatchAll(typeof(BoomDead));
            harmony.PatchAll(typeof(SpawnEnemy));
            
            // Graphics
            harmony.PatchAll(typeof(Fog));
            harmony.PatchAll(typeof(DisablePost));

            // NoBackgroundHover
            harmony.PatchAll(typeof(NoBackgroundHover));
            harmony.PatchAll(typeof(NoBackgroundHover_Stub));

            // Global Vars
            harmony.PatchAll(typeof(PlayerAvatarVars));
            harmony.PatchAll(typeof(HealthUIVars));
            harmony.PatchAll(typeof(EnergyUIVars));
        }
    }
}
