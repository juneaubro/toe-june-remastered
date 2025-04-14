using HarmonyLib;
using UnityEngine;

namespace PileOMods.Patches
{
    internal class KYS
    {
        public static ModHotkey scuiKey = new ModHotkey(MouseAndKeyboard.Delete, enableKYS, true);
        public static bool scui;

        [HarmonyPatch(typeof(PlayerHealth), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerHealth __instance)
        {
            scuiKey.Update();
            if (scui)
            {
                int maxHealth=(int)Traverse.Create(__instance).Field("maxHealth").GetValue();
                Traverse.Create(__instance).Method("Hurt",maxHealth,true);
                scui = false;
            }
        }

        public static void enableKYS()
        {
            scui = true;
            GodMode.isGodMode = false;
            Debug.Log("KYS-ing");
        }
    }
}
