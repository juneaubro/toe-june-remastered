using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using GameNetcodeStuff;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class InfiniteStamina
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void Update(PlayerControllerB __instance)
        {
            __instance.sprintMeter = float.MaxValue;
        }
    }
}
