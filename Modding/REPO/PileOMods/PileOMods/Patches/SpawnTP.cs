using HarmonyLib;
using UnityEngine;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerController))]
    internal class SpawnTP
    {
        public static ModHotkey homeTPKey = new ModHotkey(MouseAndKeyboard.Home, homeTP, true);
        public static bool TPHome = false;

        [HarmonyPatch(typeof(PlayerController), "Update")]
        [HarmonyPostfix]
        static void Update(PlayerController __instance)
        {
            homeTPKey.Update();
            if(TPHome)
            {
                __instance.transform.position = PlayerAvatarVars.spawnPos;
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
