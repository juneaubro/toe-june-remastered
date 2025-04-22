using HarmonyLib;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(Map))]
    internal class EnemyOnMap
    {
        public static Map map;

        [HarmonyPatch(typeof(Map), "Update")]
        [HarmonyPostfix]
        static void Update(Map __instance)
        {
            map = __instance;
            if (EnemyList.tmp.enabled)
            {
                AddEnemyMarkers();
            }
            else
            {

            }
        }

        public static void AddEnemyMarkers()
        {
            foreach (var i in GetEnemies())
            {
                if (i.Key.transform.Find("Sprite") == null)
                {
                    GameObject e = Object.Instantiate(map.transform.GetComponentInChildren<MapLayer>().gameObject.transform.Find("Item Cart Medium(Clone)").Find("Sprite").gameObject, i.Key.transform);
                    e.layer = map.transform.GetComponentInChildren<MapLayer>().gameObject.transform.Find("Item Cart Medium(Clone)").gameObject.layer;
                    e.name = "Sprite";
                    e.transform.localScale = Vector3.one;
                    e.GetComponent<SpriteRenderer>().color = Color.red;
                }
                //i.Key.transform.Find("Sprite").transform.position = new Vector3(i.Key.transform.Find("Enable").Find("Controller").transform.position.x, map.transform.position.y, i.Key.transform.Find("Enable").Find("Controller").transform.position.z);
            }
        }

        public static Dictionary<GameObject,int> GetEnemies()
        {
            Dictionary<GameObject, int> enemies = new Dictionary<GameObject, int>();
            foreach (Transform child in LevelGenerator.Instance.gameObject.transform.parent.GetComponentInChildren<EnemyParent>().transform.parent.transform)
            {
                if (enemies.ContainsKey(child.gameObject))
                {
                    enemies[child.gameObject] += 1;
                }
                else if (child.childCount != 0)
                {
                    enemies.Add(child.gameObject, child.gameObject.GetInstanceID());
                }
                else if (child.childCount == 0)
                {
                    continue;
                }

                if (!child.gameObject.transform.Find("Enable").gameObject.activeSelf)
                {
                    enemies.Remove(child.gameObject);
                }
            }
            return enemies;
        }
    }
}
