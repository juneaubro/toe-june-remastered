using HarmonyLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(StatsUI))]
    internal class EnemyList : SemiUI
    {
        public static GameObject enemyListUI;
        public static TextMeshProUGUI tmp;
        public static Dictionary<string, int> listOfEnemies = new Dictionary<string, int>();

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
            if (__instance.gameObject.transform.GetChild(0).gameObject.activeSelf && MainMenuOpen.instance == null)
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
                if (listOfEnemies.ContainsKey(child.gameObject.name))
                    listOfEnemies[child.gameObject.name] += 1;
                else if (child.gameObject.transform.childCount != 0)
                    listOfEnemies.Add(child.gameObject.name, 1);
            }
            foreach (var child in listOfEnemies)
            {
                enemyListUI.GetComponent<TextMeshProUGUI>().text += $"{child.Key}\n";
                enemyListUI.transform.Find("StatsNumbers").GetComponent<TextMeshProUGUI>().text += $"{child.Value}\n";
            }
            string clean = Regex.Replace(enemyListUI.GetComponent<TextMeshProUGUI>().text, @"Enemy\s*-\s*|\(Clone\)", "").Trim();
            enemyListUI.GetComponent<TextMeshProUGUI>().text = clean.ToUpper();
            listOfEnemies.Clear();
        }
    }
}
