using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class NightVisionPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(ref Light ___nightVision)
        {
            ___nightVision.enabled = true;
            ___nightVision.intensity = 100f;
            ___nightVision.range = 1000000f;
            ___nightVision.shadowStrength = 0f;
            ___nightVision.shadows = LightShadows.None;
            ___nightVision.shape = LightShape.Box;
        }
    }
}
