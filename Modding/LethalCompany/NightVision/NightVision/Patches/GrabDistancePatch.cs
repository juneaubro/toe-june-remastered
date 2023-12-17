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
            ___grabDistance = 50f;
        }
    }
}
