using HarmonyLib;
using UnityEngine;

namespace Mods.Patches
{
    [HarmonyPatch(typeof(Engine))]
    internal class EnginePatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void Awake()
        {
            Debug.Log("Testing TestMod message!!!!!!\nTesting TestMod message!!!!!!\nTesting TestMod message!!!!!!\nTesting TestMod message!!!!!!\nTesting TestMod message!!!!!!\nTesting TestMod message!!!!!!\nTesting TestMod message!!!!!!\nTesting TestMod message!!!!!!\nTesting TestMod message!!!!!!");
        }

        // Well it worked and found out the engine script starts when the game starts and has a lot of shit.
    }
}
