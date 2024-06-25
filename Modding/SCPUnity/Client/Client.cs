using System.Diagnostics;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UdpClient = NetCoreServer.UdpClient;

internal class Client : UdpClient
{
    public enum EventType
    {
        Join = 0,
        Leave = 1,
        UpdateRotation = 2,
        UpdateLocation = 3,
        StartGame = 4,
        EndGame = 5
    }

    public static Client Instance = null!;
    public bool StartupError = false;
    public Process GameProcess;
    public IntPtr GameHandle = IntPtr.Zero;
    public bool GameRunningFirst = false;

    private string _address;
    private int _port;
    private bool _stop;
    private int _retryAttempts = 0;

    public Client(string aAddress, int aPort, bool gameRunning = false) : base(aAddress, aPort)
    {
        _address = aAddress;
        _port = aPort;
        Instance = this;
        GameRunningFirst = gameRunning;

        // Game might have started first, if so,
        // current directory is already the game folder
        string gamePath = "";
        gamePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Name == "SCMP" ? $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\SCP Unity.exe"
            : $@"{Directory.GetCurrentDirectory()}\SCP Unity.exe";

        if (!GameRunningFirst)
        {
            if (File.Exists(gamePath))
            {
                Console.WriteLine("Found game exe, starting process");
                GameProcess = new Process();
                GameProcess.StartInfo.UseShellExecute = false;
                GameProcess.StartInfo.FileName = gamePath;

                if (GameProcess.Start())
                {
                    Console.WriteLine("Game process started, getting process handle");
                    GameHandle = GameProcess.Handle;

                    if (GameHandle == IntPtr.Zero)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Game process handle is not valid. Exiting.");
                        Console.ResetColor();
                        StartupError = true;
                    }
                    else
                    {
                        Console.WriteLine("Game process handle acquired");
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Game exe was not found. Make sure all files are in the BepInEx plugins directory. Exiting.");
                Console.ResetColor();
                StartupError = true;
            }
        }
        else
        {
            // search for existing game process
            Process[] processes = Process.GetProcessesByName("SCP Unity");

            foreach (Process process in processes)
            {
                Console.WriteLine("Found game process");
                GameProcess = process;
                GameHandle = process.Handle;
            }
        }
    }

    ~Client()
    {
        if (!GameProcess.HasExited)
        {
            GameProcess.Close();
            DisconnectAndStop();
        }
    }

    public void DisconnectAndStop()
    {
        _stop = true;
        Disconnect();
        while (IsConnected)
        {
            Thread.Yield();
        }
    }

    protected override void OnConnected()
    {
        if (Send($"{(int)EventType.Join}Client1\0") > 0)
        {
            Console.WriteLine($"Connected");
        }

        // Start receive datagrams
        ReceiveAsync();
    }

    protected override void OnDisconnected()
    {
        // Try to connect again
        if (!_stop)
        {
            Console.WriteLine($"Disconnected");
            Thread.Sleep(3000);
            Connect();
        }
    }

    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    {
        Console.WriteLine("Incoming: " + Encoding.UTF8.GetString(buffer, (int)offset, (int)size));

        // Continue receive datagrams
        ReceiveAsync();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Echo UDP client caught an error with code {error}");
    }

    //protected override void OnSent(EndPoint endpoint, long sent)
    //{
    //    ReceiveAsync();
    //}
}