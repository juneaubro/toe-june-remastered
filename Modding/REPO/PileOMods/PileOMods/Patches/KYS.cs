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
            int maxHealth = (int)Traverse.Create(__instance).Field("maxHealth").GetValue();
            if (scui)
            {
                __instance.HurtOther(maxHealth, __instance.transform.position, false);
                scui = false;
            }
        }

        public static void enableKYS()
        {
            GodMode.isGodMode = false;
            scui = true;
            Debug.Log("KYS-ing");
        }
    }
}
