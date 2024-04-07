using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class GodMode
    {
        public static ModHotkey godKey = new ModHotkey(MouseAndKeyboard.Home, toggleGodMode);
        public static ModHotkey godKey2 = new ModHotkey(MouseAndKeyboard.MouseForward, toggleGodMode);
        public static ModHotkey dethKey = new ModHotkey(MouseAndKeyboard.Delete, killThyself);
        public static bool isGodMode;
        static bool wantstodie;
        public static PlayerControllerB lp;

        [HarmonyPatch(typeof(PlayerControllerB),"Awake")]
        [HarmonyPostfix]
        static void Awake()
        {
            isGodMode = false;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPrefix]
        static void Update()
        {
            if(lp == null)
                lp = Player.LocalPlayer();

            godKey.Update();
            godKey2.Update();
            dethKey.Update();

            if (isGodMode && lp != null)
            {
                lp.takingFallDamage = false;
            }

            if (wantstodie && !isGodMode && lp != null)
            {
                lp.KillPlayer(Vector3.zero);
                wantstodie = false;
            }
        }

        public static void toggleGodMode()
        {
            isGodMode = !isGodMode;

            Debug.Log($"GodMode {isGodMode}");

            //if (isGodMode)
            //    Debug.Log("GodMode on.");
            //else
            //{
            //    Debug.Log("GodMode off.");
            //}
        }

        public static void killThyself()
        {
            wantstodie = true;

            Debug.Log($"Wants to die: {wantstodie}");
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
        static void setPostPlayerHitGroundEffects()
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
