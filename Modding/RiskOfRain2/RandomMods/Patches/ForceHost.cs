using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace RandomMods.Patches
{
    [HarmonyPatch(typeof(NetworkServer))]
    internal class ForceHost
    {
        public static bool hostStatus = false;

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        static void Update(ref bool ___s_Active)
        {
            ___s_Active = true;
            hostStatus = ___s_Active;
        }
    }
}
