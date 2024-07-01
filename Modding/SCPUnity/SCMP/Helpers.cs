using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace SCMP
{
    class Helpers
    {
        #region READONLY FIELDS
        private readonly string CurrentDirectory;
        private readonly string CurrentFolder;

        public readonly string GameFileName;
        public readonly string GameFilePath;

        public readonly string BinPath;

        public readonly string LobbyInfoFileName;
        public readonly string LobbyInfoFilePath;

        public readonly string ServerFileName;
        public readonly string ServerPidName;
        public readonly string ServerTxtFileName;
        public readonly string ServerPidPath;
        public readonly string ServerFilePath;
        public readonly string ServerTxtFilePath;

        public readonly string ClientFileName;
        public readonly string ClientPidName;
        public readonly string ClientPidPath;
        public readonly string ClientFilePath;
        #endregion


        private static int _level = 0;
        private static Component[] components;

        public Helpers()
        {
            CurrentDirectory = Directory.GetCurrentDirectory();
            CurrentFolder =
                CurrentDirectory.Substring(CurrentDirectory.LastIndexOf('\\') + 1);

            GameFileName = "SCP Unity.exe";
            GameFilePath = $@"{CurrentDirectory}\{GameFileName}";

            BinPath = CurrentFolder == "SCMP"
                ? $@"{CurrentDirectory}\bin\"
                : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\";

            LobbyInfoFileName = "lobbyinfo.txt";
            LobbyInfoFilePath = CurrentFolder == "SCMP"
                ? $@"{CurrentDirectory}\bin\{LobbyInfoFileName}"
                : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\{LobbyInfoFileName}";

            ServerFileName = "Server.exe";
            ServerPidName = "serverpid.txt";
            ServerTxtFileName = "server.txt";
            ServerPidPath = CurrentFolder == "SCMP"
                ? $@"{CurrentDirectory}\bin\{ServerPidName}"
                : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\{ServerPidName}";
            ServerFilePath = CurrentFolder == "SCMP"
                ? $@"{CurrentDirectory}\{ServerFileName}"
                : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\{ServerFileName}";
            ServerTxtFilePath = CurrentFolder == "SCMP"
                ? $@"{CurrentDirectory}\bin\{ServerTxtFileName}"
                : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\{ServerTxtFileName}";

            ClientFileName = "Client.exe";
            ClientPidName = "pid.txt";
            ClientPidPath = CurrentFolder == "SCMP"
                ? $@"{CurrentDirectory}\bin\{ClientPidName}"
                : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\{ClientPidName}";
            ClientFilePath = CurrentFolder == "SCMP"
                ? $@"{CurrentDirectory}\{ClientFileName}"
                : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\{ClientFileName}";
        }

        /// <summary>
        /// Recursively prints all children and components attached to the gameObject, optionally printing to a log file
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)] // Force no inlining for stack walking in Print()
        public static void PrintGameObjectInfo(GameObject gameObject, bool printToLog = true)
        {
            string tabs = "";

            if (_level == 0)
            {
                Print($"PrintGameObjectInfo: {gameObject}", printToLog);
            }
            else
            {
                // why does this work
                for (int j = 0; j < _level; j++)
                {
                    tabs += "\t";
                }
            }

            _level++;

            for (int j = 0; j < _level; j++)
            {
                tabs += "\t";
            }

            Print($"{tabs}Components:", printToLog);
            components = gameObject.GetComponents<Component>();
            foreach (Component c in components)
            {
                Print($"{tabs}\t{c.GetType()}", printToLog);
            }
            if (gameObject.transform.childCount != 0)
            {
                Print($"{tabs}Children:", printToLog);

                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    GameObject child = gameObject.transform.GetChild(i).gameObject;
                    Print($"{tabs}\t{child}", printToLog);
                    PrintGameObjectInfo(child);
                    _level--;
                }
            }

        }

        /// <summary>
        /// Prints string to console and optionally to a log file in [root game folder]/Logs/
        /// It will create a folder in the Logs folder for each new class
        /// and a seperate text file for each method name that uses the Print() method
        /// If txt file is given the wrong name, try decorating the method with [MethodImpl(MethodImplOptions.NoInlining)]
        /// </summary>
        public static void Print(string stringToPrint, bool logToOutputFile = false, LogType logType = LogType.Log,
            LogOption logOption = LogOption.None, params object[] args)
        {
            Debug.LogFormat(logType, logOption, null, stringToPrint, args);

            if (logToOutputFile)
            {
                MethodBase methodInfo;

                // using PrintGameObjectInfo while printing to a log file
                // should give the method calling it rather than PrintGameObjectInfo itself
                // r e  c   u    r     s      i       o        n
                for (int framesToSkip = 1; (methodInfo = new StackFrame(framesToSkip, true).GetMethod()).Name == "PrintGameObjectInfo"; ++framesToSkip)
                {
                    // an "interesting" way to inline methodInfo initialization without having to remake the StackFrame when broken from loop
                }

                string className = "";
                string methodName = methodInfo.Name + ".txt";

                if (methodInfo.ReflectedType != null)
                {
                    className = methodInfo.ReflectedType.Name;
                }

                string logDirectory = Directory.GetCurrentDirectory() + @"\Logs\";
                Directory.CreateDirectory(logDirectory);
                DirectoryInfo dirInfo = Directory.CreateDirectory(logDirectory + className + @"\");
                string directory = dirInfo.FullName;

                using (StreamWriter outputFile = new StreamWriter(Path.Combine(directory, methodName), true))
                {
                    outputFile.WriteLine(stringToPrint);
                    outputFile.Close();

                }
            }
        }

        /// <summary>
        /// Clears [root game folder]/Logs/ folder
        /// </summary>
        public static void ClearLogFiles()
        {
            string logDirectory = Directory.GetCurrentDirectory() + @"\Logs\";
            if (Directory.Exists(logDirectory))
                Directory.Delete(logDirectory, true);
        }

        /// <summary>
        /// Copy a component to a different GameObject if it doesn't already exist,
        /// otherwise copy all fields to the existing component on the destination GameObject
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="original">Original component to copy</param>
        /// <param name="destination">Destination GameObject to copy component to</param>
        /// <returns>Copied component</returns>
        public static T CopyComponent<T>(T original, ref GameObject destination) where T : Component
        {
            var type = original.GetType();
            var fields = type.GetFields();
            if (destination.GetComponent(type) == null)
            {
                var copy = destination.AddComponent(type);
                foreach (var field in fields)
                    field.SetValue(copy, field.GetValue(original));
                return copy as T;
            }
            var copy2 = destination.GetComponent(type);
            foreach (var field in fields)
                field.SetValue(copy2, field.GetValue(original));

            return copy2 as T;
        }

        /// <summary>
        /// Get the last character of a <see cref="string"/>
        /// </summary>
        /// <param name="input">Input <see cref="string"/></param>
        /// <returns>Last <see langword="char"/> in <see cref="string"/></returns>
        public static char GetLastCharacter(string input)
        {
            return string.IsNullOrEmpty(input) ? '\0' : input[input.Length - 1];
        }

        /// <summary>
        /// Read all bytes into a <see langword="byte[]"/> in a file until end of stream is reached
        /// </summary>
        /// <param name="filePath">File path to attempt to open</param>
        /// <returns>All bytes read as a <see cref="string"/> or <see langword="null"/></returns>
        public static string ReadFileBytes(string filePath)
        {
            byte[] buffer = null;

            try
            {
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // old school straight C style
                    int length = (int)fileStream.Length;    // get file length
                    buffer = new byte[length];              // create buffer
                    int count;                              // actual number of bytes read
                    int offset = 0;                         // total number of bytes read

                    while ((count = fileStream.Read(buffer, offset, length - offset)) > 0)
                    {
                        offset += count;
                    }

                    fileStream.Close();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

            if (buffer != null) return System.Text.Encoding.Default.GetString(buffer);
            return null;
        }

        /// <summary>
        /// Check if <see cref="File"/> is ready by attempting to open file
        /// </summary>
        /// <param name="filePath">Path to <see cref="File"/> to check</param>
        /// <param name="fileAccess"><see cref="FileAccess"/> to attempt to open file with</param>
        /// <param name="fileShare"><see cref="FileShare"/> to attempt to open file with</param>
        /// <returns><see langword="true"/> if <see cref="File"/> can opened and has bytes to read, <see langword="false"/> otherwise</returns>
        public static bool IsFileReady(string filePath = null, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.None)
        {
            if (!File.Exists(filePath) || filePath == null)
            {
                return false;
            }

            try
            {
                using (FileStream inputStream = File.Open(filePath, FileMode.Open, fileAccess, fileShare))
                {
                    if (fileAccess == FileAccess.Write)
                        return true;

                    return inputStream.Length > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Wait for file to be available
        /// </summary>
        /// <param name="filePath">Path to <see cref="File"/> to wait for</param>
        /// <param name="fileAccess"><see cref="FileAccess"/> to attempt to open file with</param>
        /// <param name="fileShare"><see cref="FileShare"/> to attempt to open file with</param>
        /// <param name="milliseconds">Time in milliseconds for <see cref="Thread"/> to sleep before checking <see cref="File"/> again</param>
        public static void WaitForFile(string filePath, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.None, int milliseconds = 1000)
        {
            while (!IsFileReady(filePath, fileAccess, fileShare))
            {
                Thread.Sleep(milliseconds);
            }
        }

        public static bool WriteToFile(string filePath, string value)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"{filePath} does not exist, creating it");
                File.Create(filePath).Close();
                Console.WriteLine($"{filePath} was created");
            }

            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

            try
            {
                Console.WriteLine($"Attempting to write to {fileName}");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(value);
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error writing to file: {e.Message}");
                return false;
            }

            Console.WriteLine($"Finished writing to {fileName}");
            return true;
        }

        public static bool WriteToFile(string filePath, params string[] values)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"{filePath} does not exist, creating it");
                File.Create(filePath).Close();
                Console.WriteLine($"{filePath} was created");
            }

            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

            try
            {
                Console.WriteLine($"Attempting to write to {fileName}");
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    foreach (string value in values)
                    {
                        writer.WriteLine($"{value},");
                    }

                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error writing to file: {e.Message}");
                return false;
            }

            Console.WriteLine($"Finished writing to {fileName}");
            return true;
        }
    }

    //public class Log : TextWriter
    //{
    //    public override Encoding Encoding { get{ return Encoding.ASCII; } }

    //    public override void WriteLine(string value)
    //    {
    //        Console.ForegroundColor = ConsoleColor.DarkRed;
    //        base.WriteLine(value);
    //        Console.ResetColor();
    //    }
    //}
    //    public enum MouseAndKeyboard
    //    {
    //        MouseLeft = -1,
    //        MouseMiddle = -2,
    //        MouseRight = -3,
    //        MouseForward = -4,
    //        MouseBack = -5,
    //        None = Key.None,
    //        Space = Key.Space,
    //        Enter = Key.Enter,
    //        Tab = Key.Tab,
    //        Backquote = Key.Backquote,
    //        Quote = Key.Quote,
    //        Semicolon = Key.Semicolon,
    //        Comma = Key.Comma,
    //        Period = Key.Period,
    //        Slash = Key.Slash,
    //        Backslash = Key.Backslash,
    //        LeftBracket = Key.LeftBracket,
    //        RightBracket = Key.RightBracket,
    //        Minus = Key.Minus,
    //        Equals = Key.Equals,
    //        A = Key.A,
    //        B = Key.B,
    //        C = Key.C,
    //        D = Key.D,
    //        E = Key.E,
    //        F = Key.F,
    //        G = Key.G,
    //        H = Key.H,
    //        I = Key.I,
    //        J = Key.J,
    //        K = Key.K,
    //        L = Key.L,
    //        M = Key.M,
    //        N = Key.N,
    //        O = Key.O,
    //        P = Key.P,
    //        Q = Key.Q,
    //        R = Key.R,
    //        S = Key.S,
    //        T = Key.T,
    //        U = Key.U,
    //        V = Key.V,
    //        W = Key.W,
    //        X = Key.X,
    //        Y = Key.Y,
    //        Z = Key.Z,
    //        Digit1 = Key.Digit1,
    //        Digit2 = Key.Digit2,
    //        Digit3 = Key.Digit3,
    //        Digit4 = Key.Digit4,
    //        Digit5 = Key.Digit5,
    //        Digit6 = Key.Digit6,
    //        Digit7 = Key.Digit7,
    //        Digit8 = Key.Digit8,
    //        Digit9 = Key.Digit9,
    //        Digit0 = Key.Digit0,
    //        LeftShift = Key.LeftShift,
    //        RightShift = Key.RightShift,
    //        LeftAlt = Key.LeftAlt,
    //        RightAlt = Key.RightAlt,
    //        AltGr = Key.AltGr,
    //        LeftCtrl = Key.LeftCtrl,
    //        RightCtrl = Key.RightCtrl,
    //        LeftMeta = Key.LeftMeta,
    //        RightMeta = Key.RightMeta,
    //        LeftWindows = Key.LeftWindows,
    //        RightWindows = Key.RightWindows,
    //        LeftApple = Key.LeftApple,
    //        RightApple = Key.RightApple,
    //        LeftCommand = Key.LeftCommand,
    //        RightCommand = Key.RightCommand,
    //        ContextMenu = Key.ContextMenu,
    //        Escape = Key.Escape,
    //        LeftArrow = Key.LeftArrow,
    //        RightArrow = Key.RightArrow,
    //        UpArrow = Key.UpArrow,
    //        DownArrow = Key.DownArrow,
    //        Backspace = Key.Backspace,
    //        PageDown = Key.PageDown,
    //        PageUp = Key.PageUp,
    //        Home = Key.Home,
    //        End = Key.End,
    //        Insert = Key.Insert,
    //        Delete = Key.Delete,
    //        CapsLock = Key.CapsLock,
    //        NumLock = Key.NumLock,
    //        PrintScreen = Key.PrintScreen,
    //        ScrollLock = Key.ScrollLock,
    //        Pause = Key.Pause,
    //        NumpadEnter = Key.NumpadEnter,
    //        NumpadDivide = Key.NumpadDivide,
    //        NumpadMultiply = Key.NumpadMultiply,
    //        NumpadPlus = Key.NumpadPlus,
    //        NumpadMinus = Key.NumpadMinus,
    //        NumpadPeriod = Key.NumpadPeriod,
    //        NumpadEquals = Key.NumpadEquals,
    //        Numpad0 = Key.Numpad0,
    //        Numpad1 = Key.Numpad1,
    //        Numpad2 = Key.Numpad2,
    //        Numpad3 = Key.Numpad3,
    //        Numpad4 = Key.Numpad4,
    //        Numpad5 = Key.Numpad5,
    //        Numpad6 = Key.Numpad6,
    //        Numpad7 = Key.Numpad7,
    //        Numpad8 = Key.Numpad8,
    //        Numpad9 = Key.Numpad9,
    //        F1 = Key.F1,
    //        F2 = Key.F2,
    //        F3 = Key.F3,
    //        F4 = Key.F4,
    //        F5 = Key.F5,
    //        F6 = Key.F6,
    //        F7 = Key.F7,
    //        F8 = Key.F8,
    //        F9 = Key.F9,
    //        F10 = Key.F10,
    //        F11 = Key.F11,
    //        F12 = Key.F12,
    //        OEM1 = Key.OEM1,
    //        OEM2 = Key.OEM2,
    //        OEM3 = Key.OEM3,
    //        OEM4 = Key.OEM4,
    //        OEM5 = Key.OEM5,
    //        IMESelected = Key.IMESelected
    //    }

    //    public class ModHotkey
    //    {
    //        public MouseAndKeyboard DefaultKey { get; set; }
    //        public MouseAndKeyboard Key { get; set; }
    //        public bool KeyWasDown { get; set; }
    //        public bool IsSettingKey { get; set; }
    //        public bool OnHold { get; set; }
    //        public bool GetKeyDown { get; set; }
    //        public Action OnKey { get; set; }

    //        public ModHotkey(MouseAndKeyboard defaultKey, Action onKey, bool onhold = false)
    //        {
    //            DefaultKey = defaultKey;
    //            Key = defaultKey;
    //            KeyWasDown = false;
    //            IsSettingKey = false;
    //            OnKey = onKey;
    //            OnHold = onhold;
    //            if (onhold)
    //            {
    //                GetKeyDown = true;
    //            }
    //        }

    //        public void Update()
    //        {
    //            // Get Mouse and Keyboard
    //            Mouse mouse = Mouse.current;
    //            Keyboard keyboard = Keyboard.current;
    //            if (mouse == null || keyboard == null)
    //            {
    //                return;
    //            }

    //            // Determine the ButtonControl
    //            ButtonControl buttonControl;
    //            switch (Key)
    //            {
    //                case MouseAndKeyboard.MouseLeft:
    //                    buttonControl = mouse.leftButton;
    //                    break;
    //                case MouseAndKeyboard.MouseMiddle:
    //                    buttonControl = mouse.middleButton;
    //                    break;
    //                case MouseAndKeyboard.MouseRight:
    //                    buttonControl = mouse.rightButton;
    //                    break;
    //                case MouseAndKeyboard.MouseForward:
    //                    buttonControl = mouse.forwardButton;
    //                    break;
    //                case MouseAndKeyboard.MouseBack:
    //                    buttonControl = mouse.backButton;
    //                    break;
    //                default:
    //                    buttonControl = keyboard[(Key)Key];
    //                    break;
    //            }

    //            // When key is pressed
    //            if (buttonControl.wasPressedThisFrame && !KeyWasDown && !IsSettingKey)
    //            {
    //                KeyWasDown = true;
    //                if (OnHold == true)
    //                    OnKey?.Invoke();
    //            }

    //            // When key is released
    //            if (buttonControl.wasReleasedThisFrame && KeyWasDown)
    //            {
    //                KeyWasDown = false;
    //                OnHold = false;
    //                OnKey?.Invoke();
    //                if (GetKeyDown)
    //                    OnHold = true;
    //            }
    //        }
    //    }

    //    public class HotkeyManager
    //    {
    //        public ModHotkey[] AllHotkeys;

    //        public HotkeyManager(int numberOfHotkeys)
    //        {
    //            AllHotkeys = new ModHotkey[numberOfHotkeys];
    //        }

    //        public bool AnyHotkeyIsSettingKey()
    //        {
    //            foreach (ModHotkey hotkey in AllHotkeys)
    //            {
    //                if (hotkey.IsSettingKey)
    //                {
    //                    return true;
    //                }
    //            }
    //            return false;
    //        }

    //        private static MouseAndKeyboard ConvertToExtendedKey(Key key)
    //        {
    //            foreach (MouseAndKeyboard extKey in Enum.GetValues(typeof(MouseAndKeyboard)))
    //            {
    //                if (Enum.GetName(typeof(Key), key) == Enum.GetName(typeof(MouseAndKeyboard), extKey))
    //                {
    //                    return extKey;
    //                }
    //            }
    //            return MouseAndKeyboard.None;
    //        }

    //        public bool SetHotKey(Key key)
    //        {
    //            MouseAndKeyboard extendedKey = ConvertToExtendedKey(key);
    //            return SetHotKey(extendedKey);
    //        }

    //        public bool SetHotKey(MouseAndKeyboard key)
    //        {
    //            foreach (ModHotkey hotkey in AllHotkeys)
    //            {
    //                if (hotkey.IsSettingKey)
    //                {
    //                    hotkey.Key = key;
    //                    ResetIsSettingKey();
    //                    return true;
    //                }
    //            }
    //            return false;
    //        }

    //        public void ResetIsSettingKey()
    //        {
    //            foreach (ModHotkey hotkey in AllHotkeys)
    //            {
    //                hotkey.IsSettingKey = false;
    //            }
    //        }

    //        public void ResetSettingKey()
    //        {
    //            foreach (ModHotkey hotkey in AllHotkeys)
    //            {
    //                hotkey.IsSettingKey = false;
    //            }
    //        }

    //        public void ResetToDefaultKey()
    //        {
    //            foreach (ModHotkey hotkey in AllHotkeys)
    //            {
    //                hotkey.Key = hotkey.DefaultKey;
    //            }
    //        }

    //        public void Update()
    //        {
    //            foreach (ModHotkey hotkey in AllHotkeys)
    //            {
    //                hotkey.Update();
    //            }
    //        }
    //    }
}