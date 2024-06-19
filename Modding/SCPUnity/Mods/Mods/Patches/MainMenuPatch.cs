using HarmonyLib;
using TheraBytes.BetterUi;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mods.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class MainMenuPatch
    {
        private static GameObject _multiplayerMenuObject = null;
        private static GameObject _buttonStack = null;
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
                CreateMultiplayerMenu();
            }
        }

        // Multiplayer button on Main Menu
        private static void CreateMultiplayerButton()
        {
            _buttonStack = _instance.mainMenuButtons.transform.GetChild(0).gameObject;

            GameObject copyObject = _buttonStack.transform.GetChild(1).gameObject;
            GameObject newObject = Object.Instantiate(copyObject);
            newObject.name = "MultiplayerButton";
            newObject.transform.SetParent(_buttonStack.transform);
            newObject.transform.localScale = copyObject.transform.localScale; // it does not automatically change this to scale after setting new parent

            Button button = newObject.transform.GetComponent<Button>();
            Button.ButtonClickedEvent newEvent = new Button.ButtonClickedEvent();
            newEvent.AddListener(OnMultiplayerClicked);
            button.onClick = newEvent;

            TextMeshProUGUI text = button.transform.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "MULTIPLAYER";
        }

        // Listener method when Multiplayer button on Main Menu is clicked
        private static void OnMultiplayerClicked()
        {
            _instance.PlayClick();

            if (!_buttonCreated)
                return;

            if(_multiplayerMenuObject == null)
                CreateMultiplayerMenu();

            ShowMultiplayerMenu(true);
        }

        private static void CreateMultiplayerMenu()
        {
            if (_multiplayerMenuObject != null)
                return;

            // MULTIPLAYER MENU OBJECT
            _multiplayerMenuObject = new GameObject
            {
                name = "MultiplayerMenuObject"
            };
            _multiplayerMenuObject.transform.SetParent(_instance.transform);
            _multiplayerMenuObject.transform.position = Vector3.zero;
            _multiplayerMenuObject.transform.localPosition = Vector3.zero;
            _multiplayerMenuObject.SetActive(false);

            // MULTIPLAYER BACK BUTTON
            GameObject backButtonObject =
                Object.Instantiate(_instance.mainMenuButtons.transform.GetChild(1).gameObject);
            backButtonObject.transform.SetParent(_multiplayerMenuObject.transform);
            // copied position is negative (????) so need to manually set its position
            backButtonObject.transform.position = new Vector3(Screen.width * 0.1f, Screen.height * 0.2f);

            Button backButton = backButtonObject.GetComponent<Button>();
            Button.ButtonClickedEvent clickedEvent = new Button.ButtonClickedEvent();
            clickedEvent.AddListener(OnBackClicked);
            backButton.onClick = clickedEvent;

            TextMeshProUGUI backText = backButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            backText.text = "BACK";

            // MULTIPLAYER MENU HEADER TEXT
            GameObject multiplayerMenuTextObject = new GameObject
            {
                name = "MultiplayerMenuText",
                transform =
                {
                    localPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.9f, 0)
                }
            };
            multiplayerMenuTextObject.transform.SetParent(_multiplayerMenuObject.transform);

            TextMeshProUGUI multiplayerText = multiplayerMenuTextObject.AddComponent<TextMeshProUGUI>();
            multiplayerText.text = "MULTIPLAYER";
            multiplayerText.color = Color.white;
            multiplayerText.fontSize = 54f;
            multiplayerText.font = backText.font;
            multiplayerText.fontMaterial = backText.fontMaterial;
            multiplayerText.alignment = TextAlignmentOptions.Center;
            multiplayerText.enableWordWrapping = false;
        }

        private static void OnBackClicked()
        {
            ShowMultiplayerMenu(false);
        }

        public static void ShowMultiplayerMenu(bool state)
        {
            _multiplayerMenuObject.SetActive(state);

            _instance.mainMenuButtons.SetActive(!state);
            _instance.mainMenuLogo.SetActive(!state);
            // need to find a way to hide/show private fields (_supportDevelopmentButton & _patreonSection)
            if (state)
            {
                // need to call this to hide the remaining private fields
                _instance.OnLoadGame();
                _instance.loadGameScreen.SetActive(false);
            }
        }
    }
}