using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(FlowermanAI))]
    internal class FlowermanUnstuck
    {// flowermanAi.inSpecialAnimationWithPlayer is the player as in PlayerControllerB instance.
        public static bool hitKillPlayer;
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(FlowermanAI __instance)
        {
            if (GodMode.isGodMode&&hitKillPlayer)
            {
                Debug.Log("Flowerman got you.");
                __instance.creatureAnimator.SetBool("carryingBody", false);
                hitKillPlayer = false;
            }
        }
    }
}
