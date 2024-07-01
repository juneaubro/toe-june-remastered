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

        private static readonly Helpers Helpers = new ();

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
                if (process.StartInfo.FileName == Helpers.ClientFilePath)
                {
                    Console.WriteLine("Client console process found");
                    _clientConsoleProcess = process;
                    Console.WriteLine($"PID : {process.Id}");
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                try
                {
                    Console.WriteLine("Client console process not found, attempting to start it");
                    _clientConsoleProcess = new Process();
                    _clientConsoleProcess.StartInfo.UseShellExecute = true;
                    _clientConsoleProcess.StartInfo.CreateNoWindow = true;
                    _clientConsoleProcess.StartInfo.FileName = Helpers.ClientFilePath;
                    _clientConsoleProcess.StartInfo.Arguments = "-gameStarted";

                    if (_clientConsoleProcess.Start())
                    {
                        Console.WriteLine("Client.exe was successfully started");
                        //Helpers.WaitForFile(pidPath);
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
            if (File.Exists(Helpers.ClientPidPath))
            {
                Helpers.WaitForFile(Helpers.ClientPidPath, FileAccess.ReadWrite);

                try
                {
                    Console.WriteLine("Getting client console PID...");
                    string pidString = Helpers.ReadFileBytes(Helpers.ClientPidPath);

                    if (int.TryParse(pidString, out int processId))
                    {
                        Console.WriteLine($"PID {processId} retrieved");
                        _clientConsoleProcess = Process.GetProcessById(processId);
                        Console.WriteLine("Successfully acquired client console handle");
                    }
                    else
                    {
                        Console.WriteLine($"Error while parsing PID from {Helpers.ClientFileName}");
                    }

                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(
                        "Could not acquire client console handle");

                    CheckForClientConsole(Helpers.ClientFilePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error reading {Helpers.ClientFileName} : {e.Message}");
                }
            }
            else
            {
                Console.WriteLine($"{Helpers.ClientFileName} not found");
                CheckForClientConsole(Helpers.ClientFilePath);

                string pidString = Helpers.ReadFileBytes(Helpers.ClientFilePath);
                if (int.TryParse(pidString, out int processId))
                {
                    _clientConsoleProcess = Process.GetProcessById(processId);
                    Console.WriteLine($"PID {processId} retrieved");
                }
                else
                {
                    Console.WriteLine($"Error while parsing PID from {Helpers.ClientFileName}");
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