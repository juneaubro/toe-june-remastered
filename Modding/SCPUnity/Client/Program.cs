using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

class Program
{
    public static Utilities Utils;
    private static string _address;
    private static int _port;
    private static string _username;
    private static bool _gameStartedFirst = false;
    private static bool _gameRunning = false;
    private static bool _quitBeforeServerInfo = false;
    private static bool _clientAlreadyExists = false;
    private static Process _gameProcess;
    private static IntPtr _gameHandle;
    private static int _previousServerPid = 0;
    private static string _previousLobbyInfo;
    private static Process[] _processes;

    static void Main(string[] args)
    {
        _processes = Process.GetProcesses();
        Utils = new Utilities();

        Console.Title = "SCMP Client";
        Console.CursorVisible = false;

        if (args.Length > 0)
        {
            foreach (string arg in args)
            {
                if (arg == "-gameStarted")
                {
                    _gameStartedFirst = true;
                }
            }
        }

        if(File.Exists(Utils.LobbyInfoFilePath))
            _previousLobbyInfo = Utils.ReadFileBytes(Utils.LobbyInfoFilePath);

        List<int> previousPids = new List<int>();

        if (!File.Exists(Utils.GamePidPath))
        {
            Console.WriteLine($"{Utils.GamePidPath} does not exist, creating it");
            File.Create(Utils.GamePidPath).Close();
        }
        else
        {
            foreach (string pid in Utils.ReadFileBytes(Utils.GamePidPath, true).Split(','))
            {
                string temp = pid;
                while (temp.Contains(','))
                    temp = temp.Remove(temp.LastIndexOf(','));

                int.TryParse(temp, out int intPid);
                if(intPid != default)
                    previousPids.Add(intPid); 
                Console.WriteLine(pid);
            }

        }

        if (!_gameStartedFirst)
        {
            Utils.WriteToFile(Utils.GamePidPath, Process.GetCurrentProcess().Id, ',');
        }

        if (File.Exists(Utils.ServerPidPath))
        {
            int.TryParse(Utils.ReadFileBytes(Utils.ServerPidPath, true), out _previousServerPid);
        }

        // point of yeeting, cache previous info before this yeets it into oblivion
        Utils.RemakeBinIfExists();

        Utils.WriteToFile(Utils.ServerPidPath, _previousServerPid);
        if(File.Exists(Utils.LobbyInfoFilePath))
            Utils.WriteToFile(Utils.LobbyInfoFilePath, _previousLobbyInfo);

        //foreach (Process process in _processes)
        //{
        //    try
        //    {
        //        if (process.MainWindowTitle == "SCMP Client" && process.Id != Process.GetCurrentProcess().Id)
        //        {
        //            _clientAlreadyExists = true;
        //            break;
        //        }
        //    }
        //    catch (InvalidOperationException e)
        //    {
        //        continue;
        //    }
        //}

        WritePid();

        if (!_gameStartedFirst)
        {
            StartGame();
        }
        else
        {
            // search for existing game process
            Process[] processes = Process.GetProcessesByName("SCP Unity");

            foreach (Process process in processes)
            {
                if (!previousPids.Contains(process.Id))
                {
                    Console.WriteLine("Found game process");
                    _gameProcess = process;
                    _gameHandle = process.Handle;
                    Utils.WaitForFile(Utils.GamePidPath, true, FileAccess.Write);
                    Utils.WriteToFile(Utils.GamePidPath, process.Id, ',');
                    break;
                }

                previousPids.Add(process.Id);
            }
        }


        _gameRunning = true;
        new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            Console.WriteLine("Game check thread started (thread will close once server info arrives)");

            while (!_gameProcess.HasExited)
            {
                Thread.Sleep(3000);
            }

            _quitBeforeServerInfo = true;
        }).Start();

        WaitForLobby();

        if (_quitBeforeServerInfo)
            return;

        Client client = new Client(_address, _port, _username, _gameRunning);

        if (client.StartupError)
        {
            Console.WriteLine("Client had an error starting up...exiting");
            Thread.Sleep(500);
            return;
        }

        Console.WriteLine("Connecting...");
        if (client.Connect())
        {
            string line = "";

            // client loop
            while (true)
            {
                if (client.GameProcess.HasExited)
                {
                    Console.WriteLine("Exiting...");
                    client.DisconnectAndStop();
                    Thread.Sleep(500);
                    break;
                }

                Thread.Sleep(1000);
            }
        }
    }

    public static void StartGame()
    {
        // Game might have started first, if so,
        // current directory is already the game folder
        //string gamePath = "";
        //gamePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Name == "SCMP" ? $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\SCP Unity.exe"
        //    : $@"{Directory.GetCurrentDirectory()}\SCP Unity.exe";

        if (File.Exists(Utils.GameFilePath))
        {
            Console.WriteLine("Found game exe, starting process");
            _gameProcess = new Process();
            _gameProcess.StartInfo.UseShellExecute = false;
            _gameProcess.StartInfo.FileName = Utils.GameFilePath;
            _gameProcess.StartInfo.Arguments = "-gameLinked";

            if (_gameProcess.Start())
            {
                Console.WriteLine("Game process started");
            }
        }
    }

    public static bool WaitForLobby()
    {
        if (!Directory.Exists(Utils.BinPath))
            Directory.CreateDirectory(Utils.BinPath);

        Console.WriteLine($"_clientAlreadyExists : {_clientAlreadyExists}");

        //if (_previousServerPid != 0 && _clientAlreadyExists)
        //{
        //    if (File.Exists(Utils.ServerTxtFilePath))
        //        File.Delete(Utils.ServerTxtFilePath);
            
        //    File.Create(Utils.ServerTxtFilePath).Close();
        //    Console.WriteLine(_previousServerPid);
        //    Utils.WriteToFile(Utils.ServerTxtFilePath, _previousServerPid);
        //}
        
        Console.WriteLine($"Waiting for server info...");

        //if (File.Exists(Utils.ServerTxtFilePath) && _clientAlreadyExists)
        //{
        //    File.Delete(Utils.ServerTxtFilePath);
        //    File.Create(Utils.ServerTxtFilePath);
        //}
        Utils.WaitForFile(Utils.ServerTxtFilePath, ref _quitBeforeServerInfo, false, FileAccess.Read, FileShare.Write);

        if (!_quitBeforeServerInfo)
        {
            string temp = Utils.ReadFileBytes(Utils.ServerTxtFilePath, true);
            Console.WriteLine(temp);
            string[] serverInfo = temp.Split(',');
            _address = serverInfo[0];
            int.TryParse(serverInfo[1], out _port);
            _username = serverInfo[2];
        }
        else
        {
            Console.WriteLine("Game process exited before retrieving server info, exiting...");
            Thread.Sleep(1000);
        }

        return !_quitBeforeServerInfo;
    }

    public static void WritePid()
    {
        if (!Directory.Exists(Utils.BinPath))
            Directory.CreateDirectory(Utils.BinPath);

        if (File.Exists(Utils.ClientPidPath))
        {
            File.Delete(Utils.ClientPidPath);
        }

        File.Create(Utils.ClientPidPath).Close();
        Utils.WriteToFile(Utils.ClientPidPath, Process.GetCurrentProcess().Id);

        // REWRITE GAME PID
        if (!_gameStartedFirst)
        {
            Utils.WriteToFile(Utils.GamePidPath, Process.GetCurrentProcess().Id, ',');
        }
    }
}