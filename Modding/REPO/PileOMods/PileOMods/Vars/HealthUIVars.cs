using HarmonyLib;
using UnityEngine;

namespace PileOMods
{
    [HarmonyPatch(typeof(HealthUI))]
    internal class HealthUIVars
    {
        public static Color maxHealthTextColorTemp;
        public static Color healthTextColorTemp;
        public static HealthUI healthUI;

        [HarmonyPatch(typeof(HealthUI), "Start")]
        [HarmonyPostfix]
        static void Start(HealthUI __instance)
        {
            maxHealthTextColorTemp = (Color) Traverse.Create(__instance).Field("textMaxHealth").Property("color").GetValue();
            healthTextColorTemp= (Color) Traverse.Create(__instance).Field("Text").Property("color").GetValue();
            healthUI = __instance;
        }
    }
}
