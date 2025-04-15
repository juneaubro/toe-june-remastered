using HarmonyLib;
using UnityEngine;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerHealth))]
    internal class GodMode
    {
        public static ModHotkey godKey = new ModHotkey(MouseAndKeyboard.MouseBack, toggleGodMode);
        public static ModHotkey fullHPKey = new ModHotkey(MouseAndKeyboard.PageUp, fullHP);
        public static ModHotkey oneHPKey = new ModHotkey(MouseAndKeyboard.PageDown, oneHP);
        public static bool isGodMode = false;
        public static bool fullHPMode = false;
        public static bool oneHPMode = false;
        public static int maxHealth;

        [HarmonyPatch(typeof(PlayerHealth), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerHealth __instance)
        {
            godKey.Update();
            fullHPKey.Update();
            oneHPKey.Update();
            maxHealth = (int)Traverse.Create(__instance).Field("maxHealth").GetValue();
            if (isGodMode)
                Traverse.Create(__instance).Field("godMode").SetValue(true);
            else
                Traverse.Create(__instance).Field("godMode").SetValue(false);
            if (fullHPMode)
            {
                Traverse.Create(__instance).Field("health").SetValue(maxHealth);
                fullHPMode = false;
            }
            if (oneHPMode)
            {
                Traverse.Create(__instance).Field("health").SetValue(1);
                oneHPMode = false;
            }
        }

        [HarmonyPatch(typeof(PlayerHealth), "Hurt")]
        [HarmonyPrefix]
        static void Hurt()
        {
            if (isGodMode)
                return;
        }

        public static void toggleGodMode()
        {
            isGodMode = !isGodMode;
            Debug.Log($"GodMode is {isGodMode}");
        }

        public static void oneHP()
        {
            oneHPMode = !oneHPMode;
            Debug.Log($"Gave 1 HP.");
        }

        public static void fullHP()
        {
            fullHPMode = !fullHPMode;
            Debug.Log($"Gave {maxHealth} HP.");
        }
    }
}
