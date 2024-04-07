using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "PlayerJump",MethodType.Enumerator)]
    internal class GodModeFallDamageFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();
            if (GodMode.isGodMode)
            {
                for (int index = 0; index < list.Count; ++index)
                { 
                    if (list[index].opcode.Name == "call")
                    {
                        list[index] = new CodeInstruction(OpCodes.Nop);
                    }
                }
            }
            return list;
        }
    }
}
// NOT A FIX :)
