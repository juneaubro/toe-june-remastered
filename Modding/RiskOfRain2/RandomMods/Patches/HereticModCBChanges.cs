using HarmonyLib;
using RoR2;
using HereticMod;

namespace RandomMods.Patches
{
    [HarmonyPatch(typeof(HereticPlugin))]
    internal class HereticModCBChanges
    {
        [HarmonyPostfix]
        [HarmonyPatch("ModifyStats")]
        static void ModifyStats(CharacterBody __0)
        {
            __0.baseMaxHealth = 440f;
            __0.levelMaxHealth = 220f;
            __0.baseDamage = 220f;
            __0.levelDamage = 90.4f;
            __0.baseRegen = 100f;
            __0.levelRegen = 0.2f;
            __0.baseCrit = 100f;
            __0.baseMoveSpeed = 25f;
            __0.baseArmor = 60f;
            __0.levelArmor = 20f;
            __0.baseAttackSpeed = 30f;
        }
    }
}
