using GameNetcodeStuff;
using HarmonyLib;
using System;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(FlowermanAI))]
    internal class FlowermanUnstuck
    {// flowermanAi.inSpecialAnimationWithPlayer is the player as in PlayerControllerB instance.
        public static Vector3 lastPosBeforeDed;
        public static PlayerControllerB pb;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update()
        {
        }

        [HarmonyPatch(typeof(FlowermanAI), "OnCollideWithPlayer")]
        [HarmonyPrefix]
        static void OnCollideWithPlayer(Collider __0,FlowermanAI __instance)
        {
            if (GodMode.isGodMode)
            {
                
                pb = __0.gameObject.GetComponent<PlayerControllerB>();
                __instance.CancelKillAnimationClientRpc((int)pb.playerClientId);
                __instance.CancelSpecialAnimationWithPlayer();
                return;
            }
        }
    }
}
