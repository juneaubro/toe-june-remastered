#nullable enable
using HarmonyLib;
using GameNetcodeStuff;

namespace NightVision
{
    internal class GlobalVars
    {
    }

    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundVars
    {
        //[HarmonyPatch("Update")]
        //[HarmonyPostfix]
        //static void Update()
        //{
        //    if (GameNetworkManager.Instance.localPlayerController != null && lp == null)
        //    {
        //        lp = GameNetworkManager.Instance.localPlayerController;
        //    }
        //}
    }
}
