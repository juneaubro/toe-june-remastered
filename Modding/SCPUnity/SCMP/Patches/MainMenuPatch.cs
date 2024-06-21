using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SCMP.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class MainMenuPatch
    {
        private static GameObject _multiplayerMenuObject = null;
        private static GameObject _buttonStack = null;
        private static GameObject _backgroundImage = null;
        private static GameObject _version = null;
        private static GameObject _officeMarketingMessage = null;
        private static GameObject _patreonButton = null;
        private static GameObject _menuButton = null;
        //private static GameObject _inputField = null;
        private static bool _buttonCreated = false;
        private static MainMenu _instance = null;

        public static GameObject HostButtonObject;
        public static Button HostButton;
        public static TextMeshProUGUI HostButtonText;
        public static GameObject JoinButtonObject;
        public static Button JoinButton;
        public static TextMeshProUGUI JoinButtonText;
        public static GameObject BackButtonObject;
        public static Button BackButton;
        public static TextMeshProUGUI BackButtonText;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Start(MainMenu __instance)
        {
            if (_instance == null)
                _instance = __instance;

            if (!_buttonCreated)
            {
                _buttonCreated = true;
                Initialize();
            }
        }

        private static void Initialize()
        {
            _buttonStack = _instance.mainMenuButtons.transform.GetChild(0).gameObject;
            _backgroundImage = _instance.transform.GetChild(0).GetChild(2).GetChild(0).gameObject;
            _version = _instance.transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
            _officeMarketingMessage = _instance.transform.GetChild(0).GetChild(1).GetChild(2).gameObject;
            _patreonButton = _instance.transform.GetChild(0).GetChild(1).GetChild(3).gameObject;
            _menuButton = _instance.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(3).GetChild(1)
                .GetChild(1).gameObject;
            //_inputField = _instance.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(3).GetChild(0).GetChild(0)
            //    .gameObject;

            SetVersionText();
            CreateMultiplayerButton();
            CreateMultiplayerMenu();
        }

        // Multiplayer button on Main Menu
        private static void CreateMultiplayerButton()
        {
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
            text.name = "MultiplayerButtonText";
            text.text = "MULTIPLAYER";
        }

        // Multiplayer menu after clicking Multiplayer button on Main Menu
        private static void CreateMultiplayerMenu()
        {
            if (_multiplayerMenuObject != null)
                return;

            // MULTIPLAYER MENU : OBJECT
            _multiplayerMenuObject = new GameObject
            {
                name = "MultiplayerMenuObject"
            };
            _multiplayerMenuObject.transform.SetParent(_instance.transform.GetChild(0));
            _multiplayerMenuObject.transform.position = Vector3.zero;
            _multiplayerMenuObject.transform.localPosition = Vector3.zero;
            _multiplayerMenuObject.SetActive(false);

            // BACKGROUND IMAGE
            _backgroundImage.transform.SetParent(_multiplayerMenuObject.transform);

            // MULTIPLAYER MENU : BACK BUTTON
            BackButtonObject =
                Object.Instantiate(_instance.mainMenuButtons.transform.GetChild(1).gameObject);
            BackButtonObject.transform.SetParent(_multiplayerMenuObject.transform);
            BackButtonObject.transform.position = new Vector3(Screen.width * 0.1f, Screen.height * 0.2f);
            BackButtonObject.name = "BackButtonObject";
            BackButton = BackButtonObject.GetComponent<Button>();
            Button.ButtonClickedEvent backButtonClickedEvent = new Button.ButtonClickedEvent();
            backButtonClickedEvent.AddListener(OnBackClicked);
            BackButton.onClick.RemoveAllListeners();
            BackButton.onClick = backButtonClickedEvent;
            BackButtonText = BackButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            BackButtonText.text = "BACK";

            // MULTIPLAYER MENU : TITLE TEXT
            GameObject titleTextObject = new GameObject
            {
                name = "MultiplayerTitleText",
                transform =
                {
                    localPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.9f, 0)
                }
            };
            titleTextObject.transform.SetParent(_multiplayerMenuObject.transform);
            TextMeshProUGUI multiplayerText = titleTextObject.AddComponent<TextMeshProUGUI>();
            multiplayerText.text = "MULTIPLAYER";
            multiplayerText.color = Color.white;
            multiplayerText.fontSize = 88f;
            multiplayerText.font = BackButtonText.font;
            multiplayerText.fontMaterial = BackButtonText.fontMaterial;
            multiplayerText.alignment = TextAlignmentOptions.Center;
            multiplayerText.enableWordWrapping = false;

            // MULTIPLAYER MENU : HOST BUTTON
            HostButtonObject = Object.Instantiate(_menuButton);
            HostButtonObject.transform.SetParent(_multiplayerMenuObject.transform);
            HostButtonObject.transform.localPosition = new Vector3(_backgroundImage.transform.localPosition.x, (Screen.height * 0.1f) + _backgroundImage.transform.localPosition.y, 0);
            HostButtonObject.transform.localScale *= 1.75f;
            HostButtonObject.name = "HostButton";
            HostButton = HostButtonObject.GetComponent<Button>();
            Button.ButtonClickedEvent hostButtonClickedEvent = new Button.ButtonClickedEvent();
            hostButtonClickedEvent.AddListener(OnHostClicked);
            HostButton.onClick.RemoveAllListeners();
            HostButton.onClick = hostButtonClickedEvent;
            HostButtonText = HostButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            HostButtonText.gameObject.name = "HostButtonText";
            HostButtonText.fontSize = 38f;
            HostButtonText.text = "HOST";

            // MULTIPLAYER MENU : JOIN BUTTON
            JoinButtonObject = Object.Instantiate(_menuButton);
            JoinButtonObject.transform.SetParent(_multiplayerMenuObject.transform);
            JoinButtonObject.transform.localPosition = new Vector3(_backgroundImage.transform.localPosition.x, (Screen.height * -0.1f) + _backgroundImage.transform.localPosition.y, 0);
            JoinButtonObject.transform.localScale *= 1.75f;
            JoinButtonObject.name = "JoinButton";
            JoinButton = JoinButtonObject.GetComponent<Button>();
            Button.ButtonClickedEvent joinButtonClickedEvent = new Button.ButtonClickedEvent();
            joinButtonClickedEvent.AddListener(OnJoinClicked);
            JoinButton.onClick.RemoveAllListeners();
            JoinButton.onClick = joinButtonClickedEvent;
            JoinButtonText = JoinButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            JoinButtonText.gameObject.name = "JoinButtonText";
            JoinButtonText.fontSize = 38f;
            JoinButtonText.text = "JOIN";
        }

        // Hide Main Menu objects, show Multiplayer Menu objects
        public static void ShowMultiplayerMenu(bool state)
        {
            _multiplayerMenuObject.SetActive(state);
            _instance.mainMenuButtons.SetActive(!state);
            _instance.mainMenuLogo.SetActive(!state);
            _officeMarketingMessage.SetActive(!state);
            _patreonButton.SetActive(!state);
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

        // Listener method when Back button on Main Menu is clicked
        private static void OnBackClicked()
        {
            ShowMultiplayerMenu(false);
            ShowHostJoinButtons(true);
        }

        private static void OnHostClicked()
        {
            ShowHostJoinButtons(false);
        }

        private static void OnJoinClicked()
        {
            ShowHostJoinButtons(false);
        }

        private static void ShowHostJoinButtons(bool state)
        {
            JoinButtonObject.SetActive(state);
            HostButtonObject.SetActive(state);
        }

        private static void SetVersionText()
        {
            _version.GetComponent<TextMeshProUGUI>().text += $"\nSCMP Version {PluginInfo.PLUGIN_VERSION}";
        }
    }
}