using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;
using NightVision;
using UnityEngine.InputSystem;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class GodMode : MonoBehaviour
    {
        public static ModHotkey godKey = new ModHotkey(MouseAndKeyboard.Home, toggleGodMode);
        public static ModHotkey dethKey = new ModHotkey(MouseAndKeyboard.Delete, killThyself);
        static bool isGodMode;
        static bool wantstodie;
        static bool canFall;
        //static int canHeal;

        [HarmonyPatch(typeof(PlayerControllerB),"Awake")]
        [HarmonyPostfix]
        static void Awake(PlayerControllerB __instance)
        {
            isGodMode = false;
            canFall = true;
            //canHeal = 0;
        }

        //[HarmonyPatch(typeof(PlayerControllerB), "Update")]
        //[HarmonyPostfix]
        //static void Update(ref int ___health)
        //{
        //    ___isPlayerDead = false;
        //    godKey.Update();
        //    if (!isGodMode)
        //        canHeal = 1;
        //    if (isGodMode)
        //    {
        //        ___health = int.MaxValue;
        //        __instance.health = int.MaxValue;
        //        setAllowPlayerDeath(ref isGodMode);
        //        setDamagePlayer();
        //        setKillPlayer
        //    }
        //    else if (!isGodMode && canHeal == 1)
        //    {
        //        ___health = 100;
        //        __instance.health = 100;
        //        canHeal = 2;
        //    }
        //    Debug.Log("GodMode " + isGodMode + ". Can Heal: " + canHeal + ". __instance.health: " + __instance.health);
        //    Debug.Log("__instance.health: " + ___health);
        //}

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        static void Update(PlayerControllerB __instance)
        {
            godKey.Update();
            if (isGodMode&&canFall)
            {
                __instance.ResetFallGravity();
                canFall = false;
            }
            dethKey.Update();
            if (wantstodie)
            {
                __instance.DamagePlayer(__instance.health);
                wantstodie = false;
            }
            Debug.Log("fallvalue: " + __instance.fallValue + ". fallvalueuncapped: " + __instance.fallValueUncapped);
        }

        public static void toggleGodMode()
        {
            isGodMode = !isGodMode;
            if (isGodMode)
                Debug.Log("GodMode on. Can Heal: " + ".toggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodMode");
            else
            {
                canFall = true;
                Debug.Log("GodMode oof. Can Heal: " + ".toggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodModetoggleGodMode");
            }
        }

        public static void killThyself()
        {
            wantstodie = true;
            Debug.Log("Wants to die: "+wantstodie);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "PlayerHitGroundEffects")]
        static void setPlayerHitGroundEffects(ref float ___fallValue,ref float ___fallValueUncapped)
        {
            if (isGodMode)
            {
                ___fallValue = 0f;
                ___fallValueUncapped = 0f;
                return;
            }
        }// suckingPower look it up in the dll

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "AllowPlayerDeath")]
        static bool setAllowPlayerDeath(ref bool __result)
        {
            if (isGodMode)
            {
                __result = false;
                return false;
            }
            __result = true;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
        static void setDamagePlayer(PlayerControllerB __instance)
        {
            if (isGodMode) 
            {
                return;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "KillPlayer")]
        static void setKillPlayer()
        {
            if(isGodMode)
                return;
        }
    }
}
