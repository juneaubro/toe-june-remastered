using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem.HID;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class WhatAreLocks
    {
        public static ModHotkey unlock = new ModHotkey(MouseAndKeyboard.H, UnlockDoor, true);
        public static bool unlockPressed = false;
        static PlayerControllerB plr;

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void Update(PlayerControllerB __instance)
        {
            plr = GameNetworkManager.Instance.localPlayerController;
            unlock.Update();
            if(unlockPressed)
            {
                RaycastHit hit;
                Vector3 ori = new Vector3(plr.transform.position.x, plr.transform.position.y, plr.transform.position.z);
                Vector3 oric = new Vector3(plr.gameplayCamera.transform.position.x, plr.gameplayCamera.transform.position.y, plr.gameplayCamera.transform.position.z);
                if (Physics.Raycast(oric + __instance.transform.forward * 1.1f, plr.gameplayCamera.transform.forward, out hit, float.MaxValue))
                {
                    if (hit.transform.gameObject.GetComponentInParent<DoorLock>() != null)
                    {
                        hit.transform.gameObject.GetComponentInParent<DoorLock>().UnlockDoorSyncWithServer();
                    }
                }
            }
        }
        static void UnlockDoor()
        {
            unlockPressed = !unlockPressed;
        }
    }
}
