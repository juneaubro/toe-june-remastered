using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(EntranceTeleport))]
    internal class TeleportInBuildingEntrance
    {
        public static ModHotkey teleKey = new ModHotkey(MouseAndKeyboard.LeftBracket, telePressedToggle);
        public static ModHotkey teleFlowerDeathKey = new ModHotkey(MouseAndKeyboard.Equals, teleFDK);
        static bool telePressed = false;
        static bool teleFDKPressed = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(EntranceTeleport __instance)
        {
            teleKey.Update();
            if (telePressed)
            {
                __instance.TeleportPlayer();
                telePressed = !telePressed;
            }
            teleFlowerDeathKey.Update();
            if (teleFDKPressed)
            {
                GodMode.lp.TeleportPlayer(FlowermanUnstuck.lastPosBeforeDed);
                teleFDKPressed = !teleFDKPressed;
            }
        }

        public static void telePressedToggle()
        {
            telePressed = !telePressed;
        }

        public static void teleFDK()
        {
            teleFDKPressed = !teleFDKPressed;
        }
    }
}
