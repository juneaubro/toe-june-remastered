using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace SCMP.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class MainMenuPatch
    {
        #region PRIVATE FIELDS
        private static GameObject _multiplayerMenuObject = null;
        private static GameObject _buttonStack = null;
        private static GameObject _backgroundImage = null;
        private static GameObject _version = null;
        private static GameObject _officeMarketingMessage = null;
        private static GameObject _patreonButton = null;
        private static GameObject _menuButton = null;
        private static GameObject _inputField = null;
        private static GameObject _newGameCopy;
        private static Button.ButtonClickedEvent _cancelButtonClickedEvent;
        private static Button.ButtonClickedEvent _startLobbyButtonClickedEvent;
        private static Button.ButtonClickedEvent _joinGameButtonClickedEvent;
        private static bool _buttonCreated = false;
        private static bool _validatingIp = false;
        private static bool _validatingPort = false;
        private static bool _validatingName = false;
        private static bool _isHost = false;
        #endregion
        
        #region PUBLIC FIELDS
        public static Process ServerProcess = null;
        public static MainMenu _instance = null;
        public static GameObject MultiplayerButtonObject;
        public static Button MultiplayerButton;
        public static TextMeshProUGUI MultiplayerButtonText;
        public static GameObject MultiplayerTitleObject;
        public static TextMeshProUGUI MultiplayerTitleText;
        public static GameObject MultiplayerBackgroundImage;
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
        public static TextMeshProUGUI IpPlaceholderText;
        public static GameObject PortInputFieldObject;
        public static TMP_InputField PortInputField;
        public static GameObject PortTextObject;
        public static TextMeshProUGUI PortText;
        public static TextMeshProUGUI PortPlaceholderText;
        public static GameObject NameInputFieldObject;
        public static TMP_InputField NameInputField;
        public static GameObject NameTextObject;
        public static TextMeshProUGUI NameText;
        public static TextMeshProUGUI NamePlaceholderText;
        public static GameObject JoinScreenNavigationButtonsObject;
        public static GameObject JoinScreenJoinButtonObject;
        public static GameObject JoinScreenCancelButtonObject;
        public static GameObject HostGameScreen;
        public static TMP_InputField HostScreenPlayerNameInputField;
        public static TextMeshProUGUI HostScreenPlayerNameInputFieldText;
        public static GameObject HostGameScreenNavigationButtons;
        public static GameObject HostGameScreenStartButtonObject;
        public static GameObject HostGameScreenCancelButtonObject;
        public static GameObject LobbyScreen;
        public static List<GameObject> LobbyPlayers;
        public static GameObject HostScreenPlayerNameObject;
        public static GameObject HostGameScreenSeed;
        public static GameObject LobbyScreenNavigationButtons;
        #endregion

        private static Helpers helpers;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Start(MainMenu __instance)
        {
            if (_instance == null)
                _instance = __instance;

            helpers = new Helpers();

            if (!_buttonCreated)
            {
                _buttonCreated = true;
                Initialize();
            }
        }

        [HarmonyPatch("OnGameLoadBegin")]
        [HarmonyPostfix]
        static void OnGameLoadBegin()
        {
            _buttonCreated = false;
        }

        private static void Initialize()
        {
            LobbyPlayers = new List<GameObject>();

            _buttonStack = _instance.mainMenuButtons.transform.GetChild(0).gameObject;
            _newGameCopy = _instance.transform.GetChild(0).GetChild(2).gameObject;
            _backgroundImage = _instance.transform.GetChild(0).GetChild(2).GetChild(0).gameObject;
            _version = _instance.transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
            _officeMarketingMessage = _instance.transform.GetChild(0).GetChild(1).GetChild(2).gameObject;
            _patreonButton = _instance.transform.GetChild(0).GetChild(1).GetChild(3).gameObject;
            _menuButton = _instance.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(3).GetChild(1)
                .GetChild(1).gameObject;
            _inputField = _instance.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(3).GetChild(0).GetChild(0)
                .gameObject;

            _cancelButtonClickedEvent = new Button.ButtonClickedEvent();
            _cancelButtonClickedEvent.AddListener(OnCancelClicked);
            _startLobbyButtonClickedEvent = new Button.ButtonClickedEvent();
            _startLobbyButtonClickedEvent.AddListener(StartLobby);
            _joinGameButtonClickedEvent = new Button.ButtonClickedEvent();
            _joinGameButtonClickedEvent.AddListener(JoinHost);

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
            MultiplayerButtonObject.transform.localScale = copyObject.transform.localScale;

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
            BackButtonObject.transform.position = new Vector3(Screen.width * 0.05f, Screen.height * 0.2f);
            BackButtonObject.name = "BackButtonObject";
            BackButton = BackButtonObject.GetComponent<Button>();
            Button.ButtonClickedEvent backButtonClickedEvent = new Button.ButtonClickedEvent();
            backButtonClickedEvent.AddListener(OnBackClicked);
            BackButton.onClick.RemoveAllListeners();
            BackButton.onClick = backButtonClickedEvent;
            BackButtonText = BackButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            BackButtonText.enableWordWrapping = false;
            BackButtonText.fontSize *= 0.825f;
            BackButtonText.text = "BACK TO MAIN MENU";

            // MULTIPLAYER MENU : TITLE TEXT
            GameObject copyObject = _instance.transform.GetChild(0).GetChild(2).GetChild(0).gameObject;
            MultiplayerTitleObject = Object.Instantiate(copyObject);
            MultiplayerTitleObject.transform.SetParent(_multiplayerMenuObject.transform);
            MultiplayerTitleObject.transform.position = copyObject.transform.position;
            MultiplayerTitleObject.name = "MultiplayerTitle";
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
            IpInputField.onValueChanged.AddListener(delegate { ValidateIp();});
            IpInputFieldObject.name = "IpInputField";
            IpPlaceholderText = IpInputFieldObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            IpPlaceholderText.text = "ex. 127.0.0.1";

            // MULTIPLAYER MENU : PORT TEXT
            PortTextObject =
                Object.Instantiate(_instance.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).gameObject);
            PortTextObject.SetActive(false);
            PortTextObject.transform.SetParent(_multiplayerMenuObject.transform);
            PortTextObject.transform.localPosition =
                new Vector3(IpTextObject.transform.localPosition.x,
                    IpTextObject.transform.localPosition.y - (Screen.height * 0.085f), 0);
            PortTextObject.name = "PortText";
            PortText = PortTextObject.GetComponent<TextMeshProUGUI>();
            PortText.text = "Port";

            // MULTIPLAYER MENU : PORT INPUT FIELD
            PortInputFieldObject = Object.Instantiate(_inputField);
            PortInputFieldObject.SetActive(false);
            PortInputFieldObject.transform.SetParent(_multiplayerMenuObject.transform);
            PortInputFieldObject.transform.localPosition = new Vector3(_backgroundImage.transform.localPosition.x,
                PortTextObject.transform.localPosition.y - (Screen.height * 0.0425f), 0);
            PortInputField = PortInputFieldObject.GetComponent<TMP_InputField>();
            PortInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            PortInputField.onValueChanged.AddListener(delegate { ValidatePort(); });
            PortInputFieldObject.name = "PortInputField";
            PortPlaceholderText =
                PortInputFieldObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            PortPlaceholderText.text = "ex. 10293";

            // MULTIPLAYER MENU : NAME TEXT
            NameTextObject =
                Object.Instantiate(_instance.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).gameObject);
            NameTextObject.SetActive(false);
            NameTextObject.transform.SetParent(_multiplayerMenuObject.transform);
            NameTextObject.transform.localPosition =
                new Vector3(IpTextObject.transform.localPosition.x,
                    PortTextObject.transform.localPosition.y - (Screen.height * 0.085f), 0);
            NameTextObject.name = "NameText";
            NameText = NameTextObject.GetComponent<TextMeshProUGUI>();
            NameText.text = "Name";

            // MULTIPLAYER MENU : NAME INPUT FIELD
            NameInputFieldObject = Object.Instantiate(_inputField);
            NameInputFieldObject.SetActive(false);
            NameInputFieldObject.transform.SetParent(_multiplayerMenuObject.transform);
            NameInputFieldObject.transform.localPosition = new Vector3(_backgroundImage.transform.localPosition.x,
                NameTextObject.transform.localPosition.y - (Screen.height * 0.0425f), 0);
            NameInputField = NameInputFieldObject.GetComponent<TMP_InputField>();
            NameInputField.onValueChanged.AddListener(delegate { ValidateName(); });
            NameInputFieldObject.name = "NameInputField";
            NamePlaceholderText =
                NameInputFieldObject.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            NamePlaceholderText.text = $"ex. SCMP_{UnityEngine.Random.RandomRangeInt(1, 1000)}";

            // MULTIPLAYER MENU : NAVIGATION BUTTONS
            GameObject navCopy = _instance.transform.GetChild(0).GetChild(2).GetChild(2).gameObject;
            JoinScreenNavigationButtonsObject = Object.Instantiate(navCopy);
            JoinScreenNavigationButtonsObject.SetActive(false);
            JoinScreenNavigationButtonsObject.transform.SetParent(_multiplayerMenuObject.transform);
            JoinScreenNavigationButtonsObject.transform.localPosition =
                new Vector3(navCopy.transform.localPosition.x, navCopy.transform.localPosition.y - (Screen.height * 0.09f), 
                    navCopy.transform.localPosition.z);
            JoinScreenNavigationButtonsObject.name = "NavigationButtons";

            // MULTIPLAYER MENU : JOIN SCREEN : JOIN LOBBY BUTTON
            JoinScreenJoinButtonObject = JoinScreenNavigationButtonsObject.transform.GetChild(0).gameObject;
            JoinScreenJoinButtonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Join Game";
            JoinScreenJoinButtonObject.GetComponent<Button>().onClick.RemoveAllListeners();
            JoinScreenJoinButtonObject.GetComponent<Button>().onClick = _joinGameButtonClickedEvent;
            JoinScreenJoinButtonObject.name = "JoinLobbyButton";

            // MULTIPLAYER MENU : JOIN SCREEN : CANCEL BUTTON
            JoinScreenCancelButtonObject = JoinScreenNavigationButtonsObject.transform.GetChild(1).gameObject;
            JoinScreenCancelButtonObject.name = "CancelButton";
            JoinScreenCancelButtonObject.GetComponent<Button>().onClick = _cancelButtonClickedEvent;

            // MULTIPLAYER MENU : HOST GAME SCREEN
            HostGameScreen = Object.Instantiate(_newGameCopy);
            HostGameScreen.SetActive(false);
            HostGameScreen.transform.SetParent(_multiplayerMenuObject.transform);
            HostGameScreen.transform.localPosition = _newGameCopy.transform.localPosition;
            HostGameScreen.name = "HostGameScreen";

            // MULTIPLAYER MENU : HOST GAME SCREEN : NAVIGATION BUTTONS
            HostGameScreenNavigationButtons = HostGameScreen.transform.GetChild(2).gameObject;
            HostGameScreenNavigationButtons.name = "HostGameScreenNavigationButtons";
            HostGameScreenNavigationButtons.transform.position = JoinScreenNavigationButtonsObject.transform.position;
            HostGameScreenNavigationButtons.transform.GetChild(0).gameObject.transform.localPosition
                = JoinScreenNavigationButtonsObject.transform.GetChild(0).gameObject.transform.localPosition;
            HostGameScreenNavigationButtons.transform.GetChild(1).gameObject.transform.localPosition
                = JoinScreenNavigationButtonsObject.transform.GetChild(1).gameObject.transform.localPosition;

            // MULTIPLAYER MENU : HOST GAME SCREEN : START LOBBY BUTTON
            HostGameScreenStartButtonObject = HostGameScreenNavigationButtons.transform.GetChild(0).gameObject;
            HostGameScreenStartButtonObject.name = "StartLobbyButton";
            HostGameScreenStartButtonObject.GetComponent<Button>().onClick.RemoveAllListeners();
            HostGameScreenStartButtonObject.GetComponent<Button>().onClick = _startLobbyButtonClickedEvent;
            HostGameScreenStartButtonObject.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = "Start Lobby";
            HostGameScreenStartButtonObject.transform.GetChild(0).name = "StartLobbyText";

            // MULTIPLAYER MENU : HOST GAME SCREEN : CANCEL BUTTON
            HostGameScreen.transform.GetChild(0).gameObject.SetActive(false);
            HostGameScreenCancelButtonObject = HostGameScreenNavigationButtons.transform.GetChild(1).gameObject;
            HostGameScreenCancelButtonObject.GetComponent<Button>().onClick.RemoveAllListeners();
            HostGameScreenCancelButtonObject.GetComponent<Button>().onClick = _cancelButtonClickedEvent;

            // MULTIPLAYER MENU : HOST GAME SCREEN : SEED
            HostGameScreenSeed = HostGameScreen.transform.GetChild(1).GetChild(3)
                .GetChild(1).gameObject;

            // MULTIPLAYER MENU : HOST GAME SCREEN : PLAYER NAME
            string dBoyRandomNumber = $"D-{UnityEngine.Random.RandomRangeInt(1, 9999)}";
            HostScreenPlayerNameObject = HostGameScreen.transform.GetChild(1).GetChild(3)
                .GetChild(0).gameObject;
            HostScreenPlayerNameInputField = HostScreenPlayerNameObject.transform.GetChild(0).gameObject
                .GetComponent<TMP_InputField>();
            HostScreenPlayerNameInputField.text = dBoyRandomNumber;
            HostScreenPlayerNameInputFieldText = HostScreenPlayerNameObject.transform.GetChild(0).GetChild(0)
                .GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            HostScreenPlayerNameInputFieldText.text = dBoyRandomNumber;

            // MULTIPLAYER MENU : LOBBY SCREEN
            LobbyScreen = new GameObject();
            LobbyScreen.name = "LobbyScreen";
            LobbyScreen.transform.SetParent(_multiplayerMenuObject.transform);

            // MULTIPLAYER MENU : LOBBY SCREEN : NAVIGATION BUTTONS
            GameObject hostNavCopy = _instance.transform.GetChild(0).GetChild(2).GetChild(2).gameObject;
            LobbyScreenNavigationButtons = Object.Instantiate(JoinScreenNavigationButtonsObject);
            LobbyScreenNavigationButtons.SetActive(false);
            LobbyScreenNavigationButtons.transform.SetParent(LobbyScreen.transform);
            LobbyScreenNavigationButtons.name = "LobbyScreenNavigationButtons";
            LobbyScreenNavigationButtons.transform.GetChild(0).GetChild(0).
                gameObject.GetComponent<TextMeshProUGUI>().text = "Start Game";
            LobbyScreenNavigationButtons.transform.GetChild(0).GetChild(0).
                gameObject.name = "StartGameText";
            LobbyScreenNavigationButtons.transform.GetChild(0).
                gameObject.name = "StartGameButton";
            LobbyScreenNavigationButtons.transform.GetChild(0).GetComponent<Button>().
                onClick.RemoveAllListeners();
            LobbyScreenNavigationButtons.transform.GetChild(1).GetComponent<Button>().
                onClick.RemoveAllListeners();
            LobbyScreenNavigationButtons.transform.GetChild(1).GetComponent<Button>().
                onClick = _cancelButtonClickedEvent;
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

            ShowStartHostGameElements(false);
            JoinScreenNavigationButtonsObject.SetActive(false);
            ShowMultiplayerMenu(true);
            ShowHostJoinButtons(true);
            ShowJoinMenu(false);
        }

        // Listener method when Back button on Main Menu is clicked
        private static void OnBackClicked()
        {
            _isHost = false;
            ShowMultiplayerMenu(false);
            ShowHostJoinButtons(true);
            LobbyScreenNavigationButtons.SetActive(false);
            JoinScreenNavigationButtonsObject.SetActive(false);
            LobbyScreen.SetActive(false);
            ShowJoinMenu(false);
        }

        // Listener method when Host button on Multiplayer menu is clicked
        private static void OnHostClicked()
        {
            _isHost = true;
            ShowHostJoinButtons(false);
            JoinScreenNavigationButtonsObject.SetActive(false);
            ShowJoinMenu(false);
            ShowStartHostGameElements(true);
        }

        // Listener method when Join button on Multiplayer menu is clicked
        private static void OnJoinClicked()
        {
            _isHost = false;
            ShowHostJoinButtons(false);
            JoinScreenNavigationButtonsObject.SetActive(true);
            ShowJoinMenu(true);
        }

        // Listener method when Cancel button on Multiplayer menu is clicked
        private static void OnCancelClicked()
        {
            _isHost = false;
            JoinScreenNavigationButtonsObject.SetActive(false);
            LobbyScreenNavigationButtons.SetActive(false);
            HostGameScreenNavigationButtons.SetActive(false);
            ShowJoinMenu(false);
            ShowHostJoinButtons(true);
            ShowStartHostGameElements(false);
            LobbyScreen.SetActive(false);
        }

        // This is probably the least efficient code I have purposefully written up until now
        // but it works
        private static void ValidateIp()
        {
            if (string.IsNullOrEmpty(IpInputField.text))
                return;

            _validatingIp = true;

            string lastCharacter = IpInputField.text[IpInputField.text.Length - 1].ToString();

            // If the last character is not a digit or a period and not a period as the first digit,
            // remove the character
            if (!Regex.IsMatch(lastCharacter, @"\d") && !Regex.IsMatch(lastCharacter, @"\.") || IpInputField.text[0] == '.')
            {
                IpInputField.text = IpInputField.text.Remove(IpInputField.text.Length - 1);
            }

            string subString = IpInputField.text;

            // If the network address (first section of an IP) number is greater than 255, remove the character
            if (uint.TryParse(subString, out uint number))
            {
                if (number > 255)
                {
                    IpInputField.text = IpInputField.text.Remove(IpInputField.text.Length - 1);
                }
            }
            else
            {
                // Get the latest IP section typed and if the number is greater than 255, remove the character
                if (IpInputField.text.Contains(".") && lastCharacter != ".")
                {
                    subString =
                        IpInputField.text.Substring(IpInputField.text.LastIndexOf('.') + 1);
                    number = 0;

                    if (uint.TryParse(subString, out number))
                    {
                        if (number > 255)
                        {
                            IpInputField.text = IpInputField.text.Remove(IpInputField.text.Length - 1);
                        }
                    }
                }

                if (lastCharacter == ".")
                {
                    // If the previous character was a period and the most recent character is a period,
                    // remove the character
                    if (IpInputField.text.Length > 2 && IpInputField.text[IpInputField.text.Length - 2] == '.')
                    {
                        IpInputField.text = IpInputField.text.Remove(IpInputField.text.Length - 1);
                    }

                    // If there are already three periods and the latest character is a period, remove the character
                    if (IpInputField.text.Split('.').Length - 1 > 3)
                    {
                        IpInputField.text = IpInputField.text.Remove(IpInputField.text.Length - 1);
                    }
                }

            }

            _validatingIp = false;
            // Should go in a OnValidateInput listener method
            //if(!Regex.IsMatch(IpInputField.text, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b") && IpInputField.text.Length > 0 && IpInputField.text != "")
            //{
            //    IpInputField.text = IpInputField.text.Remove(IpInputField.text.Length - 1);
            //}
        }

        private static void ValidatePort()
        {
            if (string.IsNullOrEmpty(PortInputField.text))
                return;

            _validatingPort = true;

            // If the parsed number is greater than the max port number or a negative,
            // remove the character
            if (uint.TryParse(PortInputField.text, out var number))
            {
                if (number > 65535)
                {
                    PortInputField.text = PortInputField.text.Remove(PortInputField.text.Length - 1);
                }
            }
            else if (PortInputField.text[PortInputField.text.Length - 1] == '-')
            {
                PortInputField.text = PortInputField.text.Remove(PortInputField.text.Length - 1);
            }

            _validatingPort = false;
        }

        private static void ValidateName()
        {
            if (string.IsNullOrEmpty(NameInputField.text))
                return;

            _validatingName = true;

            string lastCharacter = Helpers.GetLastCharacter(NameInputField.text).ToString();

            // If the last character is not a letter, digit, underscore, or the name exceeds 16 characters,
            // remove the character
            if (!Regex.IsMatch(lastCharacter, @"^\w+$") || NameInputField.text.Length > 16)
            {
                NameInputField.text = NameInputField.text.Remove(NameInputField.text.Length - 1);
            }

            _validatingName = false;
        }

        private static bool IsValidating()
        {
            return _validatingIp || _validatingPort || _validatingName;
        }

        private static void ShowHostJoinButtons(bool state)
        {
            JoinButtonObject.SetActive(state);
            HostButtonObject.SetActive(state);
        }

        private static void ShowStartHostGameElements(bool state)
        {
            HostGameScreen.SetActive(state);
            HostGameScreenNavigationButtons.SetActive(state);
            LobbyScreenNavigationButtons.SetActive(state);
        }

        private static void ShowJoinMenu(bool state)
        {
            ShowIpField(state);
            ShowPortField(state);
            ShowNameField(state);
            JoinScreenNavigationButtonsObject.SetActive(state);
        }

        private static void ShowIpField(bool state)
        {
            IpInputFieldObject.SetActive(state);
            IpTextObject.SetActive(state);
        }

        private static void ShowPortField(bool state)
        {
            PortInputFieldObject.SetActive(state);
            PortTextObject.SetActive(state);
        }

        private static void ShowNameField(bool state)
        {
            NameInputFieldObject.SetActive(state);
            NameTextObject.SetActive(state);
        }

        private static void StartLobby()
        {
            GameObject playerCopy = HostGameScreenNavigationButtons.
                transform.GetChild(0).gameObject;
            GameObject player = Object.Instantiate(playerCopy);
            player.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = 
                HostScreenPlayerNameObject.transform.GetChild(0).GetComponent<TMP_InputField>().text;
            LobbyPlayers.Add(player);


            bool foundServer = false;

            string serverPid = "";
            if (File.Exists(helpers.ServerPidPath))
            {
                Console.WriteLine($"Waiting for {helpers.ServerPidPath}");
                Helpers.WaitForFile(helpers.ServerPidPath);
            }

            if (!string.IsNullOrEmpty(serverPid))
            {
                serverPid = Helpers.ReadFileBytes(helpers.ServerPidPath);
                int.TryParse(serverPid, out int pid);

                try
                {
                    ServerProcess = Process.GetProcessById(pid);
                    Console.WriteLine("Successfully obtained server process");
                    foundServer = true;

                }
                catch (ArgumentException e)
                {
                    StartNewServer();
                }
            }
            else
            {
                StartNewServer();
            }

            // TODO: BEFORE LOADING LOBBY PLAYERS, NEED TO ADD ALREADY CONNECTED CLIENTS TO LOBBYPLAYERS FROM SERVER'S CLIENTLIST
            JoinHost();
            //LoadLobbyPlayers();
        }

        private static void StartNewServer()
        {
            Console.WriteLine($"No server process found, starting one at {helpers.ServerFilePath}");
            ServerProcess = new Process();
            ServerProcess.StartInfo.FileName = helpers.ServerFilePath;
            ServerProcess.StartInfo.UseShellExecute = true;
            ServerProcess.StartInfo.CreateNoWindow = true;
            ServerProcess.StartInfo.Arguments = "-gameStarted";

            if (ServerProcess.Start())
            {
                Console.WriteLine("Server successfully started");
            }
        }

        private static void LoadLobbyPlayers()
        {
            LobbyScreen.SetActive(true);
            ShowStartHostGameElements(false);
            ShowJoinMenu(false);
            LobbyScreenNavigationButtons.SetActive(true);
            HostGameScreen.transform.localPosition = _newGameCopy.transform.localPosition;


            int spacing = 0;
            int padding = 4;
            int playerBoxHeight = 38;

            for(int i = 0; i < LobbyScreen.transform.childCount; i++)
            {
                if(LobbyScreen.transform.GetChild(i).gameObject != null && 
                   LobbyScreen.transform.GetChild(i).gameObject != LobbyScreenNavigationButtons)
                    Object.Destroy(LobbyScreen.transform.GetChild(i).gameObject);
            }
            
            foreach (GameObject p in LobbyPlayers)
            {
                p.transform.SetParent(LobbyScreen.transform);
                p.name = "PlayerInLobby";
                p.transform.GetChild(0).gameObject.name = "PlayerInLobbyText";
                p.GetComponent<Button>().onClick.RemoveAllListeners();
                p.transform.SetParent(LobbyScreen.transform);
                p.transform.position = _backgroundImage.transform.position;
                p.transform.localPosition = new Vector3(666f, (825f - (playerBoxHeight * spacing)) + padding, 0f);
                spacing++;
            }
        }

        private static void JoinHost()
        {
            string ip = "";
            string port = "";
            string username = "";

            // If IP, port, or name are empty, give it the default value (respective placeholder text)
            if (string.IsNullOrEmpty(IpInputField.text))
            {
                IpInputField.text = IpPlaceholderText.text.Substring(IpPlaceholderText.text.IndexOf(' ') + 1);
                ip = IpInputField.text;
            } 

            if (string.IsNullOrEmpty(PortInputField.text))
            {
                PortInputField.text = PortPlaceholderText.text.Substring(PortPlaceholderText.text.IndexOf(' ') + 1);
                port = PortInputField.text;
            }

            if (string.IsNullOrEmpty(NameInputField.text) && !_isHost)
            {
                NameInputField.text = NamePlaceholderText.text.Substring(NamePlaceholderText.text.IndexOf(' ') + 1);
                username = NameInputField.text;
            } 
            else if (_isHost)
            {
                username = HostScreenPlayerNameInputFieldText.text;
            }

            //if (!_isHost)
            //    Helpers.WaitForFile(helpers.ServerTxtFilePath, FileAccess.Write);

            Helpers.WriteToFile(helpers.ServerTxtFilePath, [ip, port, username]);

            Debug.Log($"Waiting for lobby info...");

            if (!_isHost)
            {
                Helpers.WaitForFile(helpers.LobbyInfoFilePath, FileAccess.Read, FileShare.Write);
                string playerNameString = Helpers.ReadFileBytes(helpers.LobbyInfoFilePath);

                foreach (string name in playerNameString.Split(','))
                {
                    GameObject newPlayer =
                        Object.Instantiate(HostGameScreenNavigationButtons.transform.GetChild(0).gameObject);
                    newPlayer.name = name;
                    newPlayer.GetComponent<Button>().onClick.RemoveAllListeners();
                    newPlayer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
                    LobbyPlayers.Add(newPlayer);
                }

                if (File.Exists(helpers.ServerTxtFilePath))
                {
                    File.Delete(helpers.ServerTxtFilePath);
                    File.Create(helpers.ServerTxtFilePath).Close();
                }
            }
            else
            {
                int iter = 0;
                string[] playerNames = new string[LobbyPlayers.Count];

                foreach (GameObject go in LobbyPlayers)
                {
                    playerNames[iter] = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    iter++;
                }

                Console.WriteLine($"Writing [{playerNames}] to file : {helpers.LobbyInfoFilePath}");
                Helpers.WriteToFile(helpers.LobbyInfoFilePath, playerNames);
            }


            LoadLobbyPlayers();
        }

        private static void SetVersionText()
        {
            _version.GetComponent<TextMeshProUGUI>().text += $"\nSCMP Version {PluginInfo.PLUGIN_VERSION}";
        }
    }
}