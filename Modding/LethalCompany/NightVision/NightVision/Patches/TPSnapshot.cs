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
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class TPSnapshot
    {
        static Vector3 pos;
        public static ModHotkey savePos = new ModHotkey(MouseAndKeyboard.Numpad4, SavePosition);
        public static ModHotkey loadPos = new ModHotkey(MouseAndKeyboard.Numpad5, LoadPosition);

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePostfix()
        {
            savePos.Update();
            loadPos.Update();
        }

        public static void SavePosition()
        {
            pos = GameNetworkManager.Instance.localPlayerController.transform.position;
        }
        
        public static void LoadPosition()
        {
            GameNetworkManager.Instance.localPlayerController.TeleportPlayer(pos);
        }
    }
}
