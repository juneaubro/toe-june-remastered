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

        public static void SetClientConsolePID()
        {

            _filePath = $@"{Directory.GetCurrentDirectory()}\BepInEx\plugins\SCMP\pid.txt";

            if (File.Exists(_filePath))
            {
                Helpers.WaitForFile(_filePath, FileAccess.ReadWrite);

                // Try to read from pid.txt to get the processId of client console to check for
                try
                {
                    Console.WriteLine("Getting client console PID...");
                    string pidString = Helpers.ReadFileBytes(_filePath);
                    if (int.TryParse(pidString, out int processId))
                    {
                        Console.WriteLine($"PID {processId} retrieved - obtaining client console handle");
                        _clientConsoleProcess = Process.GetProcessById(processId);
                        _clientConsoleHandle = _clientConsoleProcess.Handle;
                        Console.WriteLine("Successfully acquired client console handle");
                    }
                    else
                    {
                        Console.WriteLine("Error while parsing pid from pid.txt");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error reading pid.txt : {e.Message}");
                    Console.WriteLine(_filePath);
                }
            }
            else
            {
                Console.WriteLine("pid.txt not found");
            }
        }

        private static void StartClientConsoleCheckThread()
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