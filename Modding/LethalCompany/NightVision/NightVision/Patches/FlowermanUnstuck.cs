using HarmonyLib;
using System;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(FlowermanAI))]
    internal class FlowermanUnstuck
    {// flowermanAi.inSpecialAnimationWithPlayer is the player as in PlayerControllerB instance.
        public static Vector3 lastPosBeforeDed;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(FlowermanAI __instance)
        {
            if (GodMode.isGodMode)
            {
                lastPosBeforeDed = GodMode.lp.transform.position;
            }
        }

        [HarmonyPatch(typeof(FlowermanAI), "OnCollideWithPlayer")]
        [HarmonyPrefix]
        static void OnCollideWithPlayer()
        {
            if (GodMode.isGodMode)
            {
                Debug.Log("Collided with flowerman.");
                GodMode.lp.TeleportPlayer(lastPosBeforeDed);
                return;
            }
        }
    }
}
