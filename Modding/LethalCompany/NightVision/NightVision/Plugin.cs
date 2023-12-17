using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using NightVision.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //harmony.PatchAll(typeof(NoFallDamage));
        }
    }
}
