using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class GodMode
    {
        public static ModHotkey godKey = new ModHotkey(MouseAndKeyboard.Home, toggleGodMode);
        public static ModHotkey dethKey = new ModHotkey(MouseAndKeyboard.Delete, killThyself);
        public static bool isGodMode;
        static bool wantstodie;
        public static PlayerControllerB lp;

        [HarmonyPatch(typeof(PlayerControllerB),"Awake")]
        [HarmonyPostfix]
        static void Awake(PlayerControllerB __instance)
        {
            isGodMode = false;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerControllerB __instance)
        {
            lp = __instance;
            godKey.Update();
            if (isGodMode)
            {
                __instance.takingFallDamage = false;
            }
            dethKey.Update();
            if (wantstodie&&!isGodMode)
            {
                __instance.KillPlayer(Vector3.zero);
                wantstodie = false;
            }
        }

        public static void toggleGodMode()
        {
            isGodMode = !isGodMode;
            if (isGodMode)
                Debug.Log("GodMode on.");
            else
            {
                Debug.Log("GodMode off.");

            }
        }

        public static void killThyself()
        {
            wantstodie = true;
            Debug.Log("Wants to die: "+wantstodie);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "PlayerHitGroundEffects")]
        static void setPrePlayerHitGroundEffects()
        {
            if (isGodMode)
            {
                return;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "PlayerHitGroundEffects")]
        static void setPostPlayerHitGroundEffects(PlayerControllerB __instance)
        {
            if (!isGodMode)
            {
                
            }
        }

        // suckingPower look it up in the dll

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
            if (isGodMode)
            {
                return;
            }
            Debug.Log("KillPlayer started");
        }
    }
}
