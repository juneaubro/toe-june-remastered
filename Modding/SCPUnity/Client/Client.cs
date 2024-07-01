using System.Diagnostics;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UdpClient = NetCoreServer.UdpClient;

internal class Client : UdpClient
{
    public static Client Instance = null!;
    public static Utilities Utils = new();
    public bool StartupError = false;
    public Process GameProcess;
    public IntPtr GameHandle = IntPtr.Zero;
    public bool GameRunningFirst = false;

    private string _address;
    private int _port;
    private string _username;
    private bool _stop;
    private bool _connected;
    private bool _inLobby;
    private int _retryAttempts = 0;
    private string[] _playerNames;

    public Client(string aAddress, int aPort, string aUsername, bool gameRunning = false) : base(aAddress, aPort)
    {
        _address = aAddress;
        _port = aPort;
        _username = aUsername;
        Instance = this;
        GameRunningFirst = gameRunning;
        _inLobby = true;

        if (!GameRunningFirst)
        {
            if (File.Exists(Utils.GameFilePath))
            {
                Console.WriteLine("Found game exe, starting process");
                GameProcess = new Process();
                GameProcess.StartInfo.UseShellExecute = false;
                GameProcess.StartInfo.FileName = Utils.GameFilePath;

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
        //if (File.Exists(Utils.ServerPidPath))
        //{
        //    File.Delete(Utils.ServerPidPath);
        //    File.Create(Utils.ServerPidPath);
        //}

        // Wait for server to spin up
        Utils.WaitForFile(Utils.ServerPidPath, false, FileAccess.ReadWrite, FileShare.Write, 3000);
        Thread.Sleep(1000);
        Console.WriteLine($"Server process id : {Utils.ReadFileBytes(Utils.ServerPidPath)}");

        if (Send(EventType.Acknowledge) > 0)
        {
        }

        // Force this thread to wait for server to acknowledge
        EndPoint endpoint = Endpoint;
        string recv = Receive(ref endpoint, 1);

        if (int.TryParse(recv, out int typeInt))
        {
            // if received the acknowledge, actually connected to a server
            if ((EventType)typeInt == EventType.Acknowledge)
                _connected = true;
        }

        if (!_connected)
            return;

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

            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            EventType type = (EventType)BitConverter.ToInt32(buffer, 0);

            //string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            //int.TryParse(message[0].ToString(), out var typeInt);
            //EventType type = (EventType)typeInt;

            switch (type)
            {
                case EventType.Acknowledge:
                    if (_inLobby)
                    {
                        _connected = true;
                        string playerNumberString = message.Substring(0, message.IndexOf('\0') - 1);

                        int.TryParse(playerNumberString, out var numberOfPlayers);

                        // update lobby
                        _playerNames = new string[numberOfPlayers];

                        _playerNames = message.Substring(message.IndexOf('\0') + 1).Split(',');

                        Utils.WaitForFile(Utils.LobbyInfoFilePath, false, FileAccess.Write, FileShare.ReadWrite);
                        Utils.WriteToFile(Utils.LobbyInfoFilePath, _playerNames);
                    }
                    break;
                case EventType.Join:
                    EventColors.FormatEventTypeOutput(EventType.Join);
                    Console.WriteLine($"{message.Remove(0, 1)} joined the server");
                    break;
                case EventType.LobbyInfo:
                    break;
                case EventType.Leave:
                case EventType.UpdateRotation:
                case EventType.UpdateLocation:
                case EventType.StartGame:
                case EventType.EndGame:
                case EventType.KickPlayer:
                case EventType.MessageSent:
                case EventType.MessageReceived:
                default: break;
            }
        }

        // Continue receive datagrams
        ReceiveAsync();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Client caught an error with code {error}");
    }

    private long Send(EventType eventType, string value = "")
    {
        return Send($"{(int)eventType}{value}");
    }
}