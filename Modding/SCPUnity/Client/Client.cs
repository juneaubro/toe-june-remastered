using System.Diagnostics;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UdpClient = NetCoreServer.UdpClient;

internal class Client : UdpClient
{
    public static Client Instance = null!;
    public bool StartupError = false;
    public Process GameProcess;
    public IntPtr GameHandle = IntPtr.Zero;
    public bool GameRunningFirst = false;

    private string _address;
    private int _port;
    private string _username;
    private bool _stop;
    private bool _connected;
    private int _retryAttempts = 0;

    public Client(string aAddress, int aPort, string aUsername, bool gameRunning = false) : base(aAddress, aPort)
    {
        _address = aAddress;
        _port = aPort;
        _username = aUsername;
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
        if (Send($"{(int)EventType.Acknowledge}") > 0)
        {
            //_connected = true;
        }

        // Force this thread to wait for server to acknowledge
        EndPoint endpoint = Endpoint;
        string temp = Receive(ref endpoint, 1);

        if (_stop)
            return;

        if (int.TryParse(temp, out int typeInt))
        {
            // if received the acknowledge, am actually connected to a server
            if ((EventType)typeInt == EventType.Acknowledge)
                _connected = true;
        }

        string connectMessage = $"{(int)EventType.Join}{_username}\0";
        char[] badChars =  {'\r', '\n'};

        foreach (char c in badChars)
        {
            while (connectMessage.Contains(c))
            {
                connectMessage = connectMessage.Remove(connectMessage.IndexOf(c), 1);
            }
        }

        if (Send(connectMessage) > 0 && _connected)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Connected");
            Console.ResetColor();
        }
        else
        {
            DisconnectAndStop();
        }

        // Start receive datagrams
        ReceiveAsync();
    }

    protected override void OnDisconnected()
    {
        // Try to connect again
        if (!_stop && _retryAttempts < 3)
        {
            _retryAttempts++;
            _connected = false;
            Console.WriteLine("Connection failed, retrying...");
            Thread.Sleep(5000);
            Connect();
        }

        if (!_stop && _retryAttempts >= 3)
        {
            Console.WriteLine("Can't connect to the specified server");
        }
    }

    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    {
        if (endpoint != Endpoint)
        {
            Console.WriteLine("Incoming: " + Encoding.UTF8.GetString(buffer, (int)offset, (int)size));

            string message = Encoding.Default.GetString(buffer);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            EventType type = (EventType)BitConverter.ToInt32(buffer, 0);

            switch (type)
            {
                case EventType.Acknowledge:
                    _connected = true;
                    Console.WriteLine($"_connected : {_connected}");
                    break;
                case EventType.Join:
                    break;
                case EventType.JoinLobby:
                    break;
                case EventType.Leave:
                    break;
                case EventType.LeaveLobby:
                    break;
                case EventType.UpdateRotation:
                    break;
                case EventType.UpdateLocation:
                    break;
                case EventType.StartGame:
                    break;
                case EventType.EndGame:
                    break;
                case EventType.KickPlayer:
                    break;
                case EventType.MessageSent:
                    break;
                case EventType.MessageReceived:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Continue receive datagrams
        ReceiveAsync();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Client caught an error with code {error}");
    }

    //protected override void OnSent(EndPoint endpoint, long sent)
    //{
    //    ReceiveAsync();
    //}
}