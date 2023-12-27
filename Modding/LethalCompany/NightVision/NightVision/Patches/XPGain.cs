using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class XPGain
    {
        public static ModHotkey xpgain = new ModHotkey(MouseAndKeyboard.Numpad1, xptog);
        static bool xptoggled = false;
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void Update(HUDManager __instance)
        {
            xpgain.Update();
            if(xptoggled)
                __instance.localPlayerXP = 500;
        }
        public static void xptog()
        {
            xptoggled = !xptoggled;
            Debug.Log($"xpgain is {xptoggled}");
        }
    }
}
