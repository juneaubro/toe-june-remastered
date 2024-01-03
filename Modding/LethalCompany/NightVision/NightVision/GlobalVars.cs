using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;

namespace NightVision
{
    internal class GlobalVars
    {
    }

    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundVars
    {
        public static PlayerControllerB lp;
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update()
        {
            if (GameNetworkManager.Instance.localPlayerController != null && lp == null)
            {
                lp = GameNetworkManager.Instance.localPlayerController;
            }
        }
    }
}
