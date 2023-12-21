using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;
using HarmonyLib;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class NoWeight
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void Update(PlayerControllerB __instance)
        {
            __instance.carryWeight = 0.1f;
        }
    }
}
