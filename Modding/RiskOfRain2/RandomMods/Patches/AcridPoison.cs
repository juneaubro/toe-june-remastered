using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RoR2;
using static RoR2.DotController;

namespace RandomMods.Patches
{
    [HarmonyPatch(typeof(DotController))]
    internal class AcridPoison
    {
        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        static void FixedUpdate(DotController __instance, ref DotController.DotDef[] ___dotDefs)//, ref DotController.DotStackPool ___dotStackPool)
        {
            ___dotDefs[4].damageCoefficient = 20.0f;

            // DotController.DotStack dotStack1 = DotController.dotStackPool.Request();
        }
    }
}
