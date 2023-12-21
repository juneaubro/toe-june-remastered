using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{

    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class RemoveOriginalDebugs
    {
        public static ModHotkey logKey = new ModHotkey(MouseAndKeyboard.End, DisableUnityLogger);

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update()
        {
            logKey.Update();
        }

        public static void DisableUnityLogger()
        {
            Debug.unityLogger.logEnabled = !Debug.unityLogger.logEnabled;

            Debug.Log(Debug.unityLogger.logEnabled);
        }
    }
}
