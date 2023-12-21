using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    internal class MovementSpeed
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();
            for (int index = 0; index < list.Count; ++index)
            {
                if (list[index].opcode.Name == "call")
                {
                    list[index] = new CodeInstruction(OpCodes.Nop);
                }
            }
            return list;
        }
    }
}
