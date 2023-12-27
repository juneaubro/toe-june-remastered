using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskedEnemyUnstuck
    {
        public static PlayerControllerB pb;

        [HarmonyPatch("OnCollideWithPlayer")]
        [HarmonyPrefix]
        static void OnCollideWithPlayerPrefix(Collider __0, MaskedPlayerEnemy __instance)
        {
            if (GodMode.isGodMode)
            {
                pb = __0.gameObject.GetComponent<PlayerControllerB>();
                __instance.CancelKillAnimationClientRpc((int)pb.playerClientId);
                __instance.CancelSpecialAnimationWithPlayer();
                pb.disableLookInput = false;
                pb.inSpecialInteractAnimation = false;
                pb.inAnimationWithEnemy = (EnemyAI) null;
                return;
            }
        }

        public static void FreshStart()
        {

        }
    }
}
