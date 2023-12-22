using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class RevivePlayers
    {
        public static ModHotkey resKey = new ModHotkey(MouseAndKeyboard.Numpad7, toggleRes);
        static bool toggledRes = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(StartOfRound __instance)
        {
            resKey.Update();
            if (toggledRes)
            {
                __instance.ReviveDeadPlayers();
                toggledRes = !toggledRes;
            }
        }

        [HarmonyPatch("GetPlayerSpawnPosition")]
        [HarmonyPrefix]
        static void GetPlayerSpawnPositionPrefix()
        {
            if (toggledRes)
            {
                return;
            }
        }

        [HarmonyPatch("ResetMiscValues")]
        [HarmonyPrefix]
        static void ResetMiscValuesPrefix()
        {
            return;
        }

        [HarmonyPatch("ReviveDeadPlayers")]
        [HarmonyPostfix]
        static void ReviveDeadPlayersPrefix()
        {
            GameNetworkManager.Instance.GetComponent<HUDManager>().HideHUD(false);
        }

        public static void toggleRes()
        {
            toggledRes = !toggledRes;
        }
    }
}
