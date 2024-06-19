using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mods.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class MainMenuPatch
    {
        private static GameObject buttonStack = null;
        private static bool _buttonCreated = false;
        private static MainMenu _instance = null;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Start(MainMenu __instance)
        {
            Cursor.lockState = CursorLockMode.None; // confined is pain

            if (_instance == null)
                _instance = __instance;

            if (!_buttonCreated)
            {
                _buttonCreated = true;

                CreateMultiplayerButton();
                
                //Helpers.PrintGameObjectInfo(buttonStack, true);
            }
        }

        public static void CreateMultiplayerButton()
        {
            buttonStack = _instance.mainMenuButtons.transform.GetChild(0).gameObject;

            GameObject copyObject = buttonStack.transform.GetChild(1).gameObject;
            GameObject newObject = Object.Instantiate(copyObject);

            Button button = newObject.transform.GetComponent<Button>();
            Button.ButtonClickedEvent newEvent = new Button.ButtonClickedEvent();
            newEvent.AddListener(OnMultiplayerClicked);
            button.onClick = newEvent;

            TextMeshProUGUI text = newObject.transform.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "MULTIPLAYER";

            newObject.transform.SetParent(buttonStack.transform);
            button.transform.SetParent(newObject.transform);
            text.transform.SetParent(button.transform);
            newObject.transform.position += new Vector3(-100, 0, 0);
        }

        public static void OnMultiplayerClicked()
        {
            _instance.PlayClick();
            if (!_buttonCreated)
                return;

            // Need to call this to hide private _supportDevelopmentButton & _patreonSection
            _instance.OnLoadGame();
            _instance.loadGameScreen.SetActive(false);

            ShowMultiplayerMenu();
            //Helpers.PrintGameObjectInfo(instance.gameObject, true);
        }

        public static void ShowMultiplayerMenu()
        {

        }
    }
}