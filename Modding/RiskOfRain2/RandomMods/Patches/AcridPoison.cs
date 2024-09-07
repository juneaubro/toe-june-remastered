using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.DotController;

namespace RandomMods.Patches
{
    [HarmonyPatch(typeof(DotController))]
    internal class AcridPoison
    {
        public static bool dotStarted = false;
        //static float cd = 0f;
        static float dmg = 30f;

        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        static void Awake(ref DotController.DotDef[] ___dotDefs)
        {
            dotStarted = true;
            ___dotDefs[4].damageCoefficient = dmg;
        }

        [HarmonyPostfix]
        [HarmonyPatch("InitDotCatalog")]
        static void InitDotCatalog(ref DotController.DotDef[] ___dotDefs)
        {
            ___dotDefs[4].damageCoefficient = dmg;
        }

        //[HarmonyPrefix]
        //[HarmonyPatch("AddDot")]
        //static void AddDot()
        //{
        //    ForceHost.hostStatus=
        //}

        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        static void FixedUpdate(DotController __instance, ref DotController.DotDef[] ___dotDefs)//, ref DotController.DotStackPool ___dotStackPool)
        {
            ___dotDefs[4].damageCoefficient = dmg;

            //if (cd > 5)
            //{
            //    Debug.Log($"ForceHost.hostStatus: {ForceHost.hostStatus}");
            //    Debug.Log($"NetworkServer.active: {NetworkServer.active}");
            //    //Debug.Log($"ForceHost.activeStatus: {ForceHost.activeStatus}");
            //    cd = 0;
            //}
            //else
            //{
            //    cd += UnityEngine.Time.deltaTime;
            //}
            //// DotController.DotStack dotStack1 = DotController.dotStackPool.Request();
        }
    }
}
