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
        static PlayerControllerB _plr;

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void Update()
        {
            if (Player.LocalPlayer() == null)
                return;

            if(_plr == null)
                _plr = Player.LocalPlayer();

            unlock.Update();
            if(unlockPressed)
            {
                Vector3 ori = new Vector3(_plr.transform.position.x, _plr.transform.position.y, _plr.transform.position.z);
                Vector3 oric = new Vector3(_plr.gameplayCamera.transform.position.x, _plr.gameplayCamera.transform.position.y, _plr.gameplayCamera.transform.position.z);
                if (Physics.Raycast(oric + _plr.transform.forward * 1.1f, _plr.gameplayCamera.transform.forward, out var hit, float.MaxValue))
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
