using HarmonyLib;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PostProcessing))]
    internal class DisablePost
    {
        [HarmonyPatch(typeof(PostProcessing), "Setup")]
        [HarmonyPrefix]
        static void Setup(PostProcessing __instance)
        {
            
        }
    }
}
