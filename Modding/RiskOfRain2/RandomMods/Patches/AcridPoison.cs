using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RoR2;
using UnityEngine;
using static RoR2.DotController;

namespace RandomMods.Patches
{
    [HarmonyPatch(typeof(DotController))]
    internal class AcridPoison
    {
        static float cd = 0f;
        static float dmg = 1f;

        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        static void Awake(ref DotController.DotDef[] ___dotDefs)
        {
            ___dotDefs[4].damageCoefficient = dmg;
        }

        [HarmonyPostfix]
        [HarmonyPatch("InitDotCatalog")]
        static void InitDotCatalog(ref DotController.DotDef[] ___dotDefs)
        {
            ___dotDefs[4].damageCoefficient = dmg;
        }

        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        static void FixedUpdate(DotController __instance, ref DotController.DotDef[] ___dotDefs)//, ref DotController.DotStackPool ___dotStackPool)
        {
            ___dotDefs[4].damageCoefficient = dmg;

            //if (cd > 10)
            //{
            //    Debug.Log(ForceHost.hostStatus);
            //    cd = 0;
            //}
            //else
            //{
            //    cd += UnityEngine.Time.deltaTime;
            //}
            // DotController.DotStack dotStack1 = DotController.dotStackPool.Request();
        }
    }
}
