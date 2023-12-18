using HarmonyLib;
using System;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(FlowermanAI))]
    internal class FlowermanUnstuck
    {// flowermanAi.inSpecialAnimationWithPlayer is the player as in PlayerControllerB instance.
        public static bool hitKillPlayer;
        public static Vector3 lastPosBeforeDed;
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(FlowermanAI __instance,ref Coroutine ___killAnimationCoroutine)
        {
            if (GodMode.isGodMode&&hitKillPlayer)
            {
                lastPosBeforeDed = GodMode.lp.transform.position;
                GodMode.lp.TeleportPlayer(lastPosBeforeDed);
                try
                {
                    //__instance.CancelKillAnimationClientRpc(GodMode.lp.deadBody.playerObjectId);
                    //__instance.StopAllCoroutines();
                    //__instance.FinishKillAnimation(false);
                    //Debug.Log("Spawn Despawn");
                    //Vector3 flowermanDeathPos=__instance.transform.position;
                    //__instance.KillEnemy();
                    //SpawnFlowerMan.rm.SpawnEnemyOnServer(flowermanDeathPos, 0, SpawnFlowerMan.enemyIndex);

                    // this is fucked 
                    // NullReferenceException: Object reference not set to an instance of an object Stack trace:
                    // FlowermanAI blah blah blah killAnimation erererrrrrrrorr
                }
                catch(Exception e)
                {

                }
                //__instance.CancelSpecialAnimationWithPlayer();
                Debug.Log("Flowerman tried to kill you.");
                hitKillPlayer = false;
                
            }
        }
    }
}
