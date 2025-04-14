using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerAvatar))]
    internal class SpawnTP
    {
        public static ModHotkey homeTPKey = new ModHotkey(MouseAndKeyboard.Home, homeTP);
        public static bool TPHome = false;

        [HarmonyPatch(typeof(PlayerAvatar), "FixedUpdate")]
        [HarmonyPostfix]
        static void FixedUpdate(PlayerAvatar __instance)
        {
            homeTPKey.Update();
            if(TPHome)
            {
                Vector3 spawnPos = (Vector3)Traverse.Create(__instance).Field("spawnPosition").GetValue();
                Traverse.Create(__instance).Field("rb").Method("MovePosition", spawnPos);
                TPHome=false;
            }
        }

        public static void homeTP()
        {
            TPHome=!TPHome;
            Debug.Log("TP back to ship.");
        }
    }
}
