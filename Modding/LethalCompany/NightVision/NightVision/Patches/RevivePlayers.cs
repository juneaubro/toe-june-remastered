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
                __instance.AllPlayersHaveRevivedClientRpc();
                __instance.PlayerHasRevivedServerRpc();

                toggledRes = !toggledRes;
            }
        }

        public static void toggleRes()
        {
            toggledRes = !toggledRes;
        }
    }
}
