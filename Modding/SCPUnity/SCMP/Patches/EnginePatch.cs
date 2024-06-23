using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using HarmonyLib;
using UnityEngine;


namespace SCMP.Patches
{
    [HarmonyPatch(typeof(Engine))]
    internal class EnginePatch
    {
        public static Engine Instance = null;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void Awake(Engine __instance)
        {
            if (Instance == null)
                Instance = __instance;
        }

        // Well it worked and found out the engine script starts when the game starts and has a lot of shit.
    }

}
