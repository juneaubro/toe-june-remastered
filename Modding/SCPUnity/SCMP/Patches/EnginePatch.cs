using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using HarmonyLib;
using UnityEngine;


namespace SCMP.Patches
{
    [HarmonyPatch(typeof(Engine))]
    internal class EnginePatch
    {
        public static Engine Instance = null;
        private static Process _clientConsoleProcess;
        private static IntPtr _clientConsoleHandle;
        private static string _filePath;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void Awake(Engine __instance)
        {
            SetClientConsolePID();

            if (Instance == null)
                Instance = __instance;

            Cursor.lockState = CursorLockMode.None; // confined is pain
            Helpers.ClearLogFiles();

            StartClientConsoleCheckThread();
        }

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
                    int sum = 0;                            // total number of bytes read

                    // read until Read method returns 0 (end of the stream has been reached)
                    while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    {
                        sum += count;  // sum is a buffer offset for next reading
                    }

                    fileStream.Close();
                }
            }
            catch (IOException ex)
            {
            }

            if (buffer != null) return System.Text.Encoding.Default.GetString(buffer);
            return null;
        }

        public static void SetClientConsolePID()
        {
            _filePath = $@"{Directory.GetCurrentDirectory()}\BepInEx\plugins\SCMP\pid.txt";

            if (File.Exists(_filePath))
            {
                // Read from pid.txt to get the processId of client console to check for
                try
                {
                    Console.WriteLine("Getting client console PID...");
                    string pidString = ReadFileBytes(_filePath);
                    Console.WriteLine(pidString);
                    if (int.TryParse(pidString, out int processId))
                    {
                        Console.WriteLine("PID retrieved - obtaining client console handle");
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
        // Well it worked and found out the engine script starts when the game starts and has a lot of shit.
    }

}
