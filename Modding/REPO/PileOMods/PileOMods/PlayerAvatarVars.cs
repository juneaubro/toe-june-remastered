using HarmonyLib;
using UnityEngine;

namespace PileOMods
{
    [HarmonyPatch(typeof(PlayerAvatar))]
    internal class PlayerAvatarVars
    {
        public static Vector3 spawnPos;

        [HarmonyPatch(typeof(PlayerAvatar), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerAvatar __instance)
        {
            spawnPos = (Vector3)Traverse.Create(__instance).Field("spawnPosition").GetValue();
        }
    }
}
