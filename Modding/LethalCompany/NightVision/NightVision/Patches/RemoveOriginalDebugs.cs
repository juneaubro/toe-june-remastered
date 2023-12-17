using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{

    //[HarmonyPatch(typeof(PlayerControllerB))]
    //internal class RemoveOriginalDebugs
    //{
    //    public static ModHotkey logKey = new ModHotkey(MouseAndKeyboard.End, DisableUnityLogger);

    //    [HarmonyPatch("Update")]
    //    [HarmonyPrefix]
    //    public static void Update()
    //    {
    //        logKey.Update();
    //    }

    //    public static void DisableUnityLogger()
    //    {
    //        if(Debug.unityLogger.logEnabled)
    //            Debug.unityLogger.logEnabled = false;
    //        else
    //        {
    //            Debug.unityLogger.logEnabled = true;
    //        }
    //    }
    //}
}
