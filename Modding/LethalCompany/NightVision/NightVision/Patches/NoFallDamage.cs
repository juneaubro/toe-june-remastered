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
    internal class NoFallDamage
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void setFallDamage(ref float ___minVelocityToTakeDamage)
        {
            ___minVelocityToTakeDamage = float.MaxValue;
        }
    }
}
