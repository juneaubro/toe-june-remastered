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
            if(Player.LocalPlayer() != null)
                pos = Player.LocalPlayer().transform.position;
        }
        
        public static void LoadPosition()
        {
            if(Player.LocalPlayer() != null)
                Player.LocalPlayer().TeleportPlayer(pos);
        }
    }
}
