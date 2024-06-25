//#define CUSTOM_SERVER_SEND

using System.Diagnostics;

class Program
{
    private static string _address;
    private static int _port;
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

        Client client = new Client(_address, _port, _gameRunning);

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
        WaitForFile(path);

        if (!_quitBeforeServerInfo)
        {
            string temp = ReadFileBytes(path);
            string[] serverInfo = temp.Split(',');
            _address = serverInfo[0];
            int.TryParse(serverInfo[1], out _port);
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
        WriteToFile(path, Process.GetCurrentProcess().Id);
    }

    public static void WriteToFile(string filePath, string value)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"{filePath} does not exist, creating it");
            File.Create(filePath).Close();
            Console.WriteLine($"{filePath} was created");
        }

        try
        {
            Console.WriteLine($"Attempting to write to {filePath.Substring(filePath.LastIndexOf('\\') + 1)}");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(value);
                writer.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error writing to file: {e.Message}");
        }

        Console.WriteLine($"Finished writing to {filePath.Substring(filePath.LastIndexOf('\\') + 1)}");
    }

    public static void WriteToFile(string filePath, int value)
    {
        WriteToFile(filePath, value.ToString());
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

    public static string ReadFile(string filePath)
    {
        string value = "";
        using (StreamReader file = new StreamReader(filePath))
        {
            while ((value = file.ReadLine()) != null)
            {
            }

            file.Close();
        }

        return value;
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
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
            return false;
        }

        try
        {
            using (FileStream inputStream = File.Open(filePath, FileMode.Open, fileAccess, fileShare))
                return inputStream.Length > 0;
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
        while (!IsFileReady(filePath, fileAccess, fileShare) && !_quitBeforeServerInfo)
        {
            Thread.Sleep(milliseconds);
        }
    }
}