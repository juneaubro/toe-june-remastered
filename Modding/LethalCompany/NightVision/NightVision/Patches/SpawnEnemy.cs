using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class SpawnEnemy
    {
        // toe make a better spawn all enemy patch pls
        // k but this thing is getting refactored - formerly known as SpawnFlowerman
        // get and set via RoundManager
        public static Dictionary<SpawnableEnemyWithRarity, int> enemies = new Dictionary<SpawnableEnemyWithRarity, int>();
        public static List<TextMeshProUGUI> enemyNames = new List<TextMeshProUGUI>(); // use to keep track of enemy names for later UI stuff
        public static Canvas canvas;

        public static ModHotkey spawnEnemynKey = new ModHotkey(MouseAndKeyboard.Backslash, spawnEnemy);
        public static bool pressedSpawnEnemy = false;
        public static bool getEnemyEnumIndices = false;
        public static int enemyIndex = 0;
        public static RoundManager rm = null;

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void Update(RoundManager __instance)
        {
            if(canvas == null)
                canvas = GameObject.Find("Systems/UI/Canvas/").GetComponent<Canvas>();

            if(rm == null)
                rm = __instance;

            spawnEnemynKey.Update();
            if (pressedSpawnEnemy && Player.LocalPlayer() != null)
            {
                pressedSpawnEnemy = false;
                Debug.Log("Spawned ENEMY GUB.");
                Vector3 fixedPos = Player.LocalPlayer().transform.position + Player.LocalPlayer().transform.forward * 5f;
                __instance.SpawnEnemyOnServer(fixedPos, 0, -1);
            }

            // Determine enemy indices at runtime to match current level's indices
            if (getEnemyEnumIndices)
            {
                getEnemyEnumIndices = false;
                foreach (SpawnableEnemyWithRarity enemy in __instance.currentLevel.Enemies)
                {
                    enemies[enemy] = enemyIndex;
                    TextMeshProUGUI tempText = new TextMeshProUGUI
                    {
                        text = enemy.enemyType.enemyName
                        
                    };
                    enemyNames.Add(tempText);
                    Debug.Log($"{enemy.enemyType.enemyName} : {enemyIndex}");
                    enemyIndex++;
                }
                Debug.Log($"Current level's enemy indices determined. Total number of enemies: {enemies.Count}");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("LoadNewLevel")]
        public static void LoadNewLevelPatch()
        {
            getEnemyEnumIndices = true;
        }

        public static void spawnEnemy()
        {
            if (rm == null || canvas == null)
                return;

            pressedSpawnEnemy = true;
            foreach (TextMeshProUGUI enemy in enemyNames)
            {
                GameObject newGameObject = new GameObject(enemy.text)
                {
                    transform =
                    {
                        parent = canvas.transform
                    }
                };
                TextMeshProUGUI tmp = newGameObject.AddComponent<TextMeshProUGUI>();
                tmp.text = enemy.text;
                newGameObject.SetActive(true);
            }

            //Debug.Log("Spawned ENEMY GUB.");
            //Vector3 fixedPos = GodMode.lp.transform.position + GodMode.lp.transform.forward * 5f;
            //rm.SpawnEnemyOnServer(fixedPos, 0, enemyIndex); // supposedly flowerman currently
            //pressedSpawnEnemy = false;
        }
    }
}
