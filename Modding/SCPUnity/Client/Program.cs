//#define CUSTOM_SERVER_SEND

using System.Diagnostics;

class Program
{
    private static string _address;
    private static int _port;
    private static string _username;
    private static bool _gameStartedFirst = false;
    private static bool _gameRunning = false;
    private static bool _quitBeforeServerInfo = false;
    private static Process _gameProcess;
    private static IntPtr _gameHandle;

    static void Main(string[] args)
    {
        Console.Title = "SCMP Client";
        Console.CursorVisible = false;

        if (args.Length > 0)
        {
            foreach (string arg in args)
            {
                if (arg == "-gameStarted")
                    _gameStartedFirst = true;
            }
        }

        WritePID();

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
                Console.WriteLine("Found game process");
                _gameProcess = process;
                _gameHandle = process.Handle;
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

            while (!client.GameProcess.HasExited)
            {
                // if defined, this process will not close by itself due to ReadLine not being interuptable
#if CUSTOM_SERVER_SEND
                line = Console.ReadLine();
                
                if (string.IsNullOrEmpty(line))
                    continue;
                
                if (line == "/quit")
                {
                    Console.WriteLine("Stopping client");
                    break;
                }
                
                client.Send(client.Endpoint, line);
#endif
                Thread.Sleep(500);
            }

            Console.WriteLine("Disconnecting...");
            client.DisconnectAndStop();
            Console.WriteLine("Exiting...");
            Thread.Sleep(500);
        }
    }

    public static void StartGame()
    {
        // Game might have started first, if so,
        // current directory is already the game folder
        string gamePath = "";
        gamePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Name == "SCMP" ? $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\SCP Unity.exe"
            : $@"{Directory.GetCurrentDirectory()}\SCP Unity.exe";

        if (File.Exists(gamePath))
        {
            Console.WriteLine("Found game exe, starting process");
            _gameProcess = new Process();
            _gameProcess.StartInfo.UseShellExecute = false;
            _gameProcess.StartInfo.FileName = gamePath;

            if (_gameProcess.Start())
            {
                Console.WriteLine("Game process started");
            }
        }
    }

    public static bool WaitForLobby()
    {
        string path = "server.txt";

        // if game opened first, directory is different
        if (_gameStartedFirst)
        {
            path = $@"{Directory.GetCurrentDirectory()}\BepInEx\plugins\SCMP\server.txt";
        }

        if (File.Exists(path))
            File.Delete(path);

        File.Create(path).Close();
        
        Console.WriteLine($"Waiting for server info...");
        Utilities.WaitForFile(path);

        if (!_quitBeforeServerInfo)
        {
            string temp = Utilities.ReadFileBytes(path);
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

    public static void WritePID()
    {
        string path = "pid.txt";

        // if game opened first, directory is different
        if (_gameStartedFirst)
        {
            path = $@"{Directory.GetCurrentDirectory()}\BepInEx\plugins\SCMP\pid.txt";
        }

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.Create(path).Close();
        Utilities.WriteToFile(path, Process.GetCurrentProcess().Id);
    }

    
}