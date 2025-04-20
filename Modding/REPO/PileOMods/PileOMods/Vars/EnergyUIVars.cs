using HarmonyLib;
using UnityEngine;

namespace PileOMods
{
    [HarmonyPatch(typeof(EnergyUI))]
    internal class EnergyUIVars
    {
        public static Color maxEnergyTextColorTemp;
        public static Color energyTextColorTemp;
        public static EnergyUI energyUI;

        [HarmonyPatch(typeof(EnergyUI), "Start")]
        [HarmonyPostfix]
        static void Start(EnergyUI __instance)
        {
            maxEnergyTextColorTemp = (Color)Traverse.Create(__instance).Field("textEnergyMax").Property("color").GetValue();
            energyTextColorTemp = (Color)Traverse.Create(__instance).Field("Text").Property("color").GetValue();
            energyUI = __instance;
        }
    }
}
