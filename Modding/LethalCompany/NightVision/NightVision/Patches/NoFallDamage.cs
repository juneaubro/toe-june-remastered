using GameNetcodeStuff;
using HarmonyLib;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class NoFallDamage
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void setFallDamage(ref float ___minVelocityToTakeDamage)
        {
            if (GodMode.isGodMode == true)
                ___minVelocityToTakeDamage = float.MaxValue;
        }
    }
}
