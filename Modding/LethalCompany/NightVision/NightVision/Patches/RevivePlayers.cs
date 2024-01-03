using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class RevivePlayers
    {
        public static ModHotkey resKey = new ModHotkey(MouseAndKeyboard.Numpad7, toggleRes);
        static bool toggledRes = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(StartOfRound __instance)
        {
            resKey.Update();
            if (toggledRes)
            {
                __instance.ReviveDeadPlayers();
            }
        }

        [HarmonyPatch("GetPlayerSpawnPosition")]
        [HarmonyPrefix]
        static void GetPlayerSpawnPositionPrefix()
        {
            if (toggledRes)
            {
                //return;
            }
        }

        [HarmonyPatch("AllPlayersHaveRevivedClientRpc")]
        [HarmonyPrefix]
        static void AllPlayersHaveRevivedClientRpcPrefix()
        {
            Debug.Log("AllPlayersHaveRevivedClientRpc ran");
        }


        [HarmonyPatch("ShipLeave")]
        [HarmonyPrefix]
        static void ShipLeavePrefix()
        {
            if (toggledRes)
            {
                return;
            }
        }
        
        [HarmonyPatch("PlayerHasRevivedServerRpc")]
        [HarmonyPrefix]
        static void PlayerHasRevivedServerRpcPrefix()
        {
            Debug.Log("PlayerHasRevivedServerRpc ran");
        }


        [HarmonyPatch("ResetMiscValues")]
        [HarmonyPrefix]
        static void ResetMiscValuesPrefix()
        {
            if (toggledRes)
                return;
        }

        [HarmonyPatch("ReviveDeadPlayers")]
        [HarmonyPostfix]
        static void ReviveDeadPlayersPrefix(StartOfRound __instance)
        {
            if (HUDManager.Instance != null)
                HUDManager.Instance.HideHUD(false);
            __instance.livingPlayers = __instance.connectedPlayersAmount + 1;
            toggledRes=!toggledRes;
        }

        [HarmonyPatch("SetShipReadyToLand")]
        [HarmonyPrefix]
        static void SetShipReadyToLandPrefix()
        {
            
        }

        [HarmonyPatch("EndOfGame")]
        [HarmonyPostfix]
        static void EndOfGamePostfix()
        {
            
        }

        public static void toggleRes()
        {
            toggledRes = !toggledRes;
        }
    }
}
