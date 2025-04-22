using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerController))]
    internal class SpawnEnemy : MonoBehaviour
    {
        public static ModHotkey SpawnEnemyKey = new ModHotkey(MouseAndKeyboard.RightArrow, Pressed, false);
        private static GameObject _enemyParent;

        [HarmonyPatch(typeof(PlayerController), "Update")]
        [HarmonyPostfix]
        public static void Update()
        {
            SpawnEnemyKey.Update();
        }

        public static void Pressed()
        {
            Console.WriteLine($"called thing 0");
            if (SemiFunc.IsMainMenu())
                return;

            Console.WriteLine($"called thing 1");
            Ray ray = SemiFunc.MainCamera().ScreenPointToRay(InputManager.instance.GetMousePosition());

            Console.WriteLine($"called thing 2");
            _enemyParent = LevelGenerator.Instance.gameObject.transform.root.GetComponentInChildren<EnemyParent>().transform.parent.gameObject;

            Console.WriteLine($"called thing 3");
            int index = Random.Range(0, _enemyParent.transform.childCount - 1);

            Console.WriteLine($"called thing 5\nindex: {index}\t{_enemyParent.transform.GetChild(index).gameObject.ToString()}");
            GameObject chosenEnemy = Object.Instantiate(_enemyParent.transform.GetChild(index).gameObject);
            //chosenEnemy.GetComponentInChildren<EnemyParent>().difficulty

            Console.WriteLine($"called thing 6\n{chosenEnemy.GetComponent<EnemyParent>().enemyName}");
            chosenEnemy.GetComponentInChildren<Enemy>().TeleportToPoint(0.1f, float.MaxValue - 1.0f);
            Console.WriteLine($"called thing 7");
            //EnemyDirector.instance.enemiesDifficulty3[0].spawnObjects[0]
            //SemiFunc.EnemySpawn(chosenEnemy.GetComponentInChildren<Enemy>());
        }
    }
}
