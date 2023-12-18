using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class SpawnFlowerMan
    {
        /*      
         *      EnemyIndex
         *     -1   -   Random
         *      0   -   Centipede
         *      1   -   Bunker Spider
         *      2   -   Hoarding bug
         *      3   -   Flowerman
         *      4   -   Crawler
         *      5   -   Blob
         *      6   -   Spring
         *      7   -   Puffer
         */

        // toe make a better spawn all enemy patch pls

        public static ModHotkey spawnFlowermanKey = new ModHotkey(MouseAndKeyboard.Backslash, spawnFlowerman);
        public static bool pressedSpawnFlowerMan = false;
        public static int enemyIndex = 3;
        public static RoundManager rm;

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void Update(RoundManager __instance)
        {
            rm = __instance;
            spawnFlowermanKey.Update();
            if (pressedSpawnFlowerMan)
            {
                Debug.Log("Spawned ENEMY GUB.");
                Vector3 fixedPos = GodMode.lp.transform.position+GodMode.lp.transform.forward*5f;
                __instance.SpawnEnemyOnServer(fixedPos, 0,enemyIndex);
                pressedSpawnFlowerMan = false;
            }
        }

        public static void spawnFlowerman()
        {
            pressedSpawnFlowerMan = true;
        }
    }
}
