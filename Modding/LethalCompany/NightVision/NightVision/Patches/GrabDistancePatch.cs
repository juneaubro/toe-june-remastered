using System;
using GameNetcodeStuff;
using HarmonyLib;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class GrabDistancePatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(ref float ___grabDistance)
        {
            if (Math.Abs(___grabDistance - 50.0f) > 0.01)
                ___grabDistance = 50.0f;
        }
    }
}
