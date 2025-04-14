using UnityEngine;
using HarmonyLib;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerAvatar))]
    internal class GrabDistance
    {
        [HarmonyPatch(typeof(PlayerAvatar), "FixedUpdate")]
        [HarmonyPrefix]
        static void FixedUpdate(PlayerAvatar __instance)
        {
            __instance.physGrabber.grabRange = 12f;
            __instance.physGrabber.grabReleaseDistance = 28f;
        }
    }
}
