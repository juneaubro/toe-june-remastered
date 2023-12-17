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
    internal class GrabDistancePatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(ref float ___grabDistance)
        {
            ___grabDistance = 50f;
        }
    }
}
