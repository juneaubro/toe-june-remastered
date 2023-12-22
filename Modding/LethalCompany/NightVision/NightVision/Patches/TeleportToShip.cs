﻿using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class TeleportToShip
    {
        public static ModHotkey teleShipKey = new ModHotkey(MouseAndKeyboard.RightBracket, teleToggle);
        static bool telePressed = false;
        
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(StartOfRound __instance)
        {
            teleShipKey.Update();
            if (telePressed)
            {
                // ship railing
                //GodMode.lp.transform.position=__instance.outsideShipSpawnPosition.position;
                // inside ship
                //GodMode.lp.transform.position=__instance.playerSpawnPositions[Random.Range(0,4)].transform.position;
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(__instance.playerSpawnPositions[0].transform.position);
                GameNetworkManager.Instance.localPlayerController.isInsideFactory = false;
                telePressed = !telePressed;
            }
        }

        public static void teleToggle()
        {
            telePressed=!telePressed;
        }
    }
}
