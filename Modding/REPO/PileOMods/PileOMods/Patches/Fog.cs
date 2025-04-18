using HarmonyLib;
using UnityEngine;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(EnvironmentDirector))]
    internal class Fog
    {
        [HarmonyPatch(typeof(EnvironmentDirector), "Update")]
        [HarmonyPostfix]
        static void Update(EnvironmentDirector __instance)
        {
            RenderSettings.fog = false;
            Traverse.Create(__instance).Field("MainCamera").Property("farClipPlane").SetValue(100f);
        }
    }
}
