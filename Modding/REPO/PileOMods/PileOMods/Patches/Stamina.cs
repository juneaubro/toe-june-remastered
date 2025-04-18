using HarmonyLib;
using UnityEngine;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerController))]
    internal class Stamina
    {
        public static ModHotkey stamKey = new ModHotkey(MouseAndKeyboard.MouseForward, toggleStamina);
        public static bool isAthlete = false;
        public static float originalEnergySprintDrain;
        public static float originalSprintRechargeAmount;

        [HarmonyPatch(typeof(PlayerController), "Start")]
        [HarmonyPostfix]
        static void Start(PlayerController __instance)
        {
            originalEnergySprintDrain = __instance.EnergySprintDrain;
            originalSprintRechargeAmount = (float) Traverse.Create(__instance).Field("sprintRechargeAmount").GetValue();
        }

        [HarmonyPatch(typeof(PlayerController),"Update")]
        [HarmonyPostfix]
        static void Update(PlayerController __instance)
        {
            stamKey.Update();
            if (isAthlete)
            {
                __instance.EnergySprintDrain = 0.5f;
                Traverse.Create(__instance).Field("sprintRechargeAmount").SetValue(5f);
                Traverse.Create(EnergyUIVars.energyUI).Field("textEnergyMax").Property("color").SetValue(Color.white);
                Traverse.Create(EnergyUIVars.energyUI).Field("Text").Property("color").SetValue(Color.white);
            }
            else
            {
                __instance.EnergySprintDrain = originalEnergySprintDrain;
                Traverse.Create(__instance).Field("sprintRechargeAmount").SetValue(originalSprintRechargeAmount);
                Traverse.Create(EnergyUIVars.energyUI).Field("textEnergyMax").Property("color").SetValue(EnergyUIVars.maxEnergyTextColorTemp);
                Traverse.Create(EnergyUIVars.energyUI).Field("Text").Property("color").SetValue(EnergyUIVars.energyTextColorTemp);
            }
        }

        public static void toggleStamina()
        {
            isAthlete = !isAthlete;
            Debug.Log($"Athelete Stamina is {isAthlete}");
        }
    }
}
