﻿using System.Text.RegularExpressions;
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
        private static GameObject _inputField = null;
        private static bool _buttonCreated = false;
        private static bool _hostClicked = false;
        private static MainMenu _instance = null;

        public static GameObject MultiplayerButtonObject;
        public static Button MultiplayerButton;
        public static TextMeshProUGUI MultiplayerButtonText;
        public static GameObject MultiplayerTitleObject;
        public static TextMeshProUGUI MultiplayerTitleText;
        public static GameObject HostButtonObject;
        public static Button HostButton;
        public static TextMeshProUGUI HostButtonText;
        public static GameObject JoinButtonObject;
        public static Button JoinButton;
        public static TextMeshProUGUI JoinButtonText;
        public static GameObject BackButtonObject;
        public static Button BackButton;
        public static TextMeshProUGUI BackButtonText;
        public static GameObject IpInputFieldObject;
        public static TMP_InputField IpInputField;
        public static GameObject IpTextObject;
        public static TextMeshProUGUI IpText;
        public static GameObject NavigationButtons;

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
            _inputField = _instance.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(3).GetChild(0).GetChild(0)
                .gameObject;

            SetVersionText();
            CreateMultiplayerButton();
            CreateMultiplayerMenu();
        }

        // Multiplayer button on Main Menu
        private static void CreateMultiplayerButton()
        {
            GameObject copyObject = _buttonStack.transform.GetChild(1).gameObject;
            MultiplayerButtonObject = Object.Instantiate(copyObject);
            MultiplayerButtonObject.name = "MultiplayerButton";
            MultiplayerButtonObject.transform.SetParent(_buttonStack.transform);
            MultiplayerButtonObject.transform.localScale = copyObject.transform.localScale; // it does not automatically change this to scale after setting new parent

            MultiplayerButton = MultiplayerButtonObject.transform.GetComponent<Button>();
            Button.ButtonClickedEvent newEvent = new Button.ButtonClickedEvent();
            newEvent.AddListener(OnMultiplayerClicked);
            MultiplayerButton.onClick = newEvent;

            MultiplayerButtonText = MultiplayerButton.transform.GetComponentInChildren<TextMeshProUGUI>();
            MultiplayerButtonText.name = "MultiplayerButtonText";
            MultiplayerButtonText.text = "MULTIPLAYER";
        }

        // Multiplayer menu after clicking Multiplayer button on Main Menu
        private static void CreateMultiplayerMenu()
        {
            if (_multiplayerMenuObject != null)
                return;

            // MULTIPLAYER MENU : OBJECT
            _multiplayerMenuObject = new GameObject
            {
                name = "MultiplayerMenu"
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
            GameObject copyObject = _instance.transform.GetChild(0).GetChild(2).GetChild(0).gameObject;
            MultiplayerTitleObject = Object.Instantiate(copyObject);
            MultiplayerTitleObject.transform.SetParent(_multiplayerMenuObject.transform);
            MultiplayerTitleObject.transform.position = copyObject.transform.position;
            MultiplayerTitleText = MultiplayerTitleObject.GetComponent<TextMeshProUGUI>();
            MultiplayerTitleText.transform.localPosition = new Vector3(copyObject.transform.localPosition.x, 
                copyObject.transform.localPosition.y - (Screen.height * 0.065f), copyObject.transform.localPosition.z);
            MultiplayerTitleText.enableWordWrapping = false;
            MultiplayerTitleText.text = "MULTIPLAYER";


            // MULTIPLAYER MENU : HOST BUTTON
            HostButtonObject = Object.Instantiate(_menuButton);
            HostButtonObject.transform.SetParent(_multiplayerMenuObject.transform);
            HostButtonObject.transform.localPosition = new Vector3(_backgroundImage.transform.localPosition.x, 
                (Screen.height * 0.1f) + _backgroundImage.transform.localPosition.y, 0);
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
            JoinButtonObject.transform.localPosition = new Vector3(_backgroundImage.transform.localPosition.x, 
                (Screen.height * -0.1f) + _backgroundImage.transform.localPosition.y, 0);
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

            // MULTIPLAYER MENU : IP TEXT
            IpTextObject =
                Object.Instantiate(_instance.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).gameObject);
            IpTextObject.SetActive(false);
            IpTextObject.transform.SetParent(_multiplayerMenuObject.transform);
            IpTextObject.transform.localPosition =
                _instance.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).localPosition;
            IpTextObject.name = "IpText";
            IpText = IpTextObject.GetComponent<TextMeshProUGUI>();
            IpText.text = "Server Address";

            // MULTIPLAYER MENU : IP INPUT FIELD
            IpInputFieldObject = Object.Instantiate(_inputField);
            IpInputFieldObject.SetActive(false);
            IpInputFieldObject.transform.SetParent(_multiplayerMenuObject.transform);
            IpInputFieldObject.transform.localPosition = new Vector3(_backgroundImage.transform.localPosition.x, 
                IpTextObject.transform.localPosition.y - (Screen.height * 0.0425f), 0);
            IpInputField = IpInputFieldObject.GetComponent<TMP_InputField>();
            IpInputField.onValueChanged.AddListener(delegate { ValidateIP();});
            IpInputFieldObject.name = "IpInputField";
            TextMeshProUGUI ipPlaceholderText =
                IpInputFieldObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            ipPlaceholderText.text = "ex. 111.33.222.244";


            // MULTIPLAYER MENU : NAVIGATION BUTTONS
            NavigationButtons = Object.Instantiate(_instance.transform.GetChild(0).GetChild(2).GetChild(2).gameObject);
            NavigationButtons.transform.SetParent(_multiplayerMenuObject.transform);
            NavigationButtons.transform.localPosition =
                _instance.transform.GetChild(0).GetChild(2).GetChild(2).localPosition;
            Button.ButtonClickedEvent cancelEvent = new Button.ButtonClickedEvent();
            cancelEvent.AddListener(OnCancelClicked);
            NavigationButtons.transform.GetChild(1).GetComponent<Button>().onClick = cancelEvent;
            NavigationButtons.SetActive(false);
            NavigationButtons.name = "NavigationButtons";

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

            NavigationButtons.SetActive(false);
            IpInputFieldObject.SetActive(false);
            ShowHostJoinButtons(true);
            ShowMultiplayerMenu(true);
        }

        // Listener method when Back button on Main Menu is clicked
        private static void OnBackClicked()
        {
            ShowMultiplayerMenu(false);
            ShowHostJoinButtons(true);
            IpInputFieldObject.SetActive(false);
            IpTextObject.SetActive(false);
            NavigationButtons.SetActive(false);
        }

        private static void OnHostClicked()
        {
            ShowHostJoinButtons(false);
            NavigationButtons.SetActive(true);
            ChangeNavigationButtonText("Start Game");
        }

        private static void OnJoinClicked()
        {
            ShowHostJoinButtons(false);
            NavigationButtons.SetActive(true);
            IpInputFieldObject.SetActive(true);
            IpTextObject.SetActive(true);
            ChangeNavigationButtonText("Join Game");
        }

        private static void OnCancelClicked()
        {
            NavigationButtons.SetActive(false);
            IpInputFieldObject.SetActive(false);
            IpTextObject.SetActive(false);
            ShowHostJoinButtons(true);
        }

        private static void ValidateIP()
        {
            if (string.IsNullOrEmpty(IpInputField.text))
                return;

            if(!Regex.IsMatch(IpInputField.text, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b") && IpInputField.text.Length > 0 && IpInputField.text != "")
            {
                IpInputField.text = IpInputField.text.Remove(IpInputField.text.Length - 1);
            }
        }

        private static void ShowHostJoinButtons(bool state)
        {
            JoinButtonObject.SetActive(state);
            HostButtonObject.SetActive(state);
        }

        private static void ChangeNavigationButtonText(string text)
        {
            NavigationButtons.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        }

        private static void SetVersionText()
        {
            _version.GetComponent<TextMeshProUGUI>().text += $"\nSCMP Version {PluginInfo.PLUGIN_VERSION}";
        }
    }
}