using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(StartMatchLever))]
    internal class StartLever
    {
        public static ModHotkey startGameKey = new ModHotkey(MouseAndKeyboard.Numpad9, StartGame);
        private static bool toggleStartGame = false;
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void Update(StartMatchLever __instance)
        {
            startGameKey.Update();
            if(toggleStartGame)
                __instance.StartGame();
        }
        public static void StartGame()
        {
            toggleStartGame = !toggleStartGame;
        }
    }
}
