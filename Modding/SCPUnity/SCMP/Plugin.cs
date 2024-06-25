using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SCMP.Patches;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System;
using UnityEngine;

namespace SCMP
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        private static Plugin Instance;

        internal static ManualLogSource mls;

        private static Process _clientConsoleProcess;
        private static IntPtr _clientConsoleHandle;
        private static string _filePath;
        private static string _fileName;
        static string PLUGIN_PATH = @$"{Directory.GetCurrentDirectory()}\BepInEx\plugins\SCMP\";

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            mls.LogInfo($"{PluginInfo.PLUGIN_GUID} started loading.");

            // Base
            harmony.PatchAll(typeof(Plugin));

            //Debug
            harmony.PatchAll(typeof(MainMenuPatch));
            harmony.PatchAll(typeof(EnginePatch));

            Helpers.ClearLogFiles();

            // Do console stuff
            SetClientConsolePID();
            StartClientConsoleCheckThread();
        }

        public void CheckForClientConsole(string pidPath)
        {
            bool found = false;
            Console.WriteLine("Checking for client console process");
            Process[] processes = Process.GetProcessesByName("WindowsTerminal");

            foreach (Process process in processes)
            {
                if (process.StartInfo.FileName == @$"{PLUGIN_PATH}\Client.exe")
                {
                    Console.WriteLine("Client console process found");
                    _clientConsoleProcess = process;
                    _clientConsoleHandle = process.Handle;
                    Console.WriteLine($"PID : {process.Id}");
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                try
                {
                    Console.WriteLine("Client console process not found, attempting to start Client.exe");
                    _clientConsoleProcess = new Process();
                    _clientConsoleProcess.StartInfo.UseShellExecute = true;
                    _clientConsoleProcess.StartInfo.CreateNoWindow = true;
                    _clientConsoleProcess.StartInfo.FileName = $@"{PLUGIN_PATH}\Client.exe";
                    _clientConsoleProcess.StartInfo.Arguments = "-gameStarted";

                    if (_clientConsoleProcess.Start())
                    {
                        _clientConsoleHandle = _clientConsoleProcess.Handle;
                        Console.WriteLine("Client.exe was successfully started - attempting to set client console PID again");
                        Helpers.WaitForFile(pidPath);
                        //SetClientConsolePID();
                    }
                }
                catch (Exception ee)
                {
                    Console.WriteLine("Couldn't start Client exe, exiting");
                    Application.Quit();
                }
            }
        }

        public void SetClientConsolePID()
        {
            string binDirectory = $@"{Directory.GetCurrentDirectory()}\bin\";

            if (new DirectoryInfo(Directory.GetCurrentDirectory()).Name != "SCMP")
            {
                binDirectory = $@"{Directory.GetCurrentDirectory()}\BepInEx\plugins\SCMP\bin\";
            }

            _filePath = $@"{binDirectory}pid.txt";
            _fileName = _filePath.Substring(_filePath.LastIndexOf('\\') + 1);

            if (!Directory.Exists(binDirectory))
                Directory.CreateDirectory(binDirectory);

            if (File.Exists(_filePath))
            {
                Helpers.WaitForFile(_filePath, FileAccess.ReadWrite);

                try
                {
                    Console.WriteLine("Getting client console PID...");
                    string pidString = Helpers.ReadFileBytes(_filePath);

                    if (int.TryParse(pidString, out int processId))
                    {
                        Console.WriteLine($"PID {processId} retrieved");
                        _clientConsoleProcess = Process.GetProcessById(processId);
                        _clientConsoleHandle = _clientConsoleProcess.Handle;
                        Console.WriteLine("Successfully acquired client console handle");
                    }
                    else
                    {
                        Console.WriteLine($"Error while parsing PID from {_fileName}");
                    }

                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(
                        "Could not acquire client console handle");

                    CheckForClientConsole(_filePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error reading {_fileName} : {e.Message}");
                }
            }
            else
            {
                Console.WriteLine($"{_fileName} not found");
                CheckForClientConsole(_filePath);

                string pidString = Helpers.ReadFileBytes(_filePath);
                if (int.TryParse(pidString, out int processId))
                {
                    Console.WriteLine($"PID {processId} retrieved");
                    _clientConsoleProcess = Process.GetProcessById(processId);
                    _clientConsoleHandle = _clientConsoleProcess.Handle;
                    Console.WriteLine("Successfully acquired client console handle");
                }
                else
                {
                    Console.WriteLine($"Error while parsing PID from {_fileName}");
                }
                //File.Create(_filePath);
            }
        }

        private void StartClientConsoleCheckThread()
        {
            Console.WriteLine("Starting thread to check for client console");

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Console.WriteLine("Client console check thread started");

                while (!_clientConsoleProcess.HasExited)
                {
                    Thread.Sleep(1000);
                }

                Console.WriteLine("Client console closed - exiting game");
                Application.Quit(); // quit the unity way because bepinex likes to take control of the process
            }).Start();
        }
    }
}