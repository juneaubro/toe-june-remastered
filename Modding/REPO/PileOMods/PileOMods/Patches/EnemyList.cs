using HarmonyLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;
using TMPro;
using UnityEngine;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(StatsUI))]
    internal class EnemyList : SemiUI
    {
        public static GameObject enemyListUI;
        public static TextMeshProUGUI tmp;
        public static Dictionary<string,TupleDict> tloe = new Dictionary<string,TupleDict>();
        public static Dictionary<GameObject,float> timers = new Dictionary<GameObject, float>();

        [HarmonyPatch(typeof(StatsUI), "Start")]
        [HarmonyPostfix]
        static void Start(StatsUI __instance)
        {
            if (GameObject.Find("EnemyList") == null)
            {
                enemyListUI = Object.Instantiate(__instance.gameObject);
                GameObject.Destroy(enemyListUI.GetComponent<StatsUI>());
                enemyListUI.gameObject.name = "EnemyList";
                enemyListUI.transform.SetParent(__instance.gameObject.transform.parent);
                enemyListUI.transform.localPosition = new Vector3(230.2819f, 71.1358f, 0f);
                tmp = enemyListUI.GetComponent<TextMeshProUGUI>();
                enemyListUI.transform.Find("Upgrades Header").GetComponent<TextMeshProUGUI>().text = "Enemies".ToUpper();
                foreach (Transform child in __instance.gameObject.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        [HarmonyPatch(typeof(StatsUI), "Update")]
        [HarmonyPostfix]
        static void Update(StatsUI __instance)
        {
            if (__instance.gameObject.transform.GetChild(0).gameObject.activeSelf && LevelGenerator.Instance.gameObject.transform.root.GetComponentInChildren<EnemyParent>()!=null)
            {
                tmp.enabled = true;
                enemyListUI.transform.Find("Upgrades Header").GetComponent<TextMeshProUGUI>().enabled = true;
                foreach (Transform child in enemyListUI.transform)
                {
                    child.gameObject.SetActive(true);
                }
                fetch();
            }
            else
            {
                enemyListUI.transform.Find("Upgrades Header").GetComponent<TextMeshProUGUI>().enabled = false;
                foreach (Transform child in enemyListUI.transform)
                {
                    child.gameObject.SetActive(false);
                }
                tmp.enabled = false;
            }
        }

        public static void fetch()
        {
            enemyListUI.GetComponent<TextMeshProUGUI>().text = "";
            enemyListUI.transform.Find("StatsNumbers").GetComponent<TextMeshProUGUI>().text = "";
            foreach (Transform child in LevelGenerator.Instance.gameObject.transform.parent.GetComponentInChildren<EnemyParent>().transform.parent.transform)
            {
                if (tloe.ContainsKey(child.gameObject.name))
                {
                    tloe[child.gameObject.name].v += new Vector2(1, 0);
                }
                else if (child.childCount != 0)
                {
                    tloe.Add(child.gameObject.name, new TupleDict(child.gameObject,child.gameObject.name,new Vector2(1, 0)));
                }
                else if(child.childCount == 0)
                {
                    continue;
                }

                if (!child.gameObject.transform.Find("Enable").gameObject.activeSelf)
                {
                    tloe[child.gameObject.name].v -= new Vector2(1, 0);
                }
                if (tloe[child.gameObject.name].v.x <= 0)
                {
                    tloe[child.gameObject.name].v = new Vector2(tloe[child.gameObject.name].v.x,child.gameObject.GetComponent<EnemyParent>().DespawnedTimer);
                    if (timers.ContainsKey(child.gameObject))
                    {
                        timers[child.gameObject] = child.gameObject.GetComponent<EnemyParent>().DespawnedTimer;
                    }
                    else
                    {
                        timers.Add(child.gameObject, child.gameObject.GetComponent<EnemyParent>().DespawnedTimer);
                    }
                }
            }
            foreach (var child in tloe)
            {
                enemyListUI.GetComponent<TextMeshProUGUI>().text += $"{child.Key}";
                if (child.Value.v.y != 0)
                {
                    float lowestTimer = float.MaxValue;
                    foreach (var timer in timers)
                    {
                        if (timer.Key.name == child.Value.str)
                        {
                            if (timer.Value < lowestTimer)
                            {
                                lowestTimer = timer.Value;
                                child.Value.v.y = lowestTimer;
                            }
                        }
                    }
                    enemyListUI.GetComponent<TextMeshProUGUI>().text += $" {ConvertToMinutesSeconds(lowestTimer)}\n";
                } else if (child.Value.v.y == 0)
                {
                    enemyListUI.GetComponent<TextMeshProUGUI>().text += $"\n";
                }
                if (child.Value.v.x == 0)
                {
                    enemyListUI.transform.Find("StatsNumbers").GetComponent<TextMeshProUGUI>().text += $"-\n";
                }
                else
                {
                    enemyListUI.transform.Find("StatsNumbers").GetComponent<TextMeshProUGUI>().text += $"{child.Value.v.x}\n";
                }
            }
            string clean = Regex.Replace(enemyListUI.GetComponent<TextMeshProUGUI>().text, @"Enemy\s*-\s*|\(Clone\)", "").Trim();
            enemyListUI.GetComponent<TextMeshProUGUI>().text = clean.ToUpper();
            tloe.Clear();
            timers.Clear();
        }

        public static string ConvertToMinutesSeconds(float totalSeconds)
        {
            int minutes = Mathf.FloorToInt(totalSeconds / 60f);
            int seconds = Mathf.FloorToInt(totalSeconds % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public class TupleDict
    {
        public GameObject go;
        public string str;
        public Vector2 v;

        public TupleDict(GameObject go, string str, Vector2 v)
        {
            this.go = go;
            this.str = str;
            this.v = v;
        }
    }
}
