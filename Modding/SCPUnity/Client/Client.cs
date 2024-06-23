using System.Diagnostics;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UdpClient = NetCoreServer.UdpClient;

internal class Client : UdpClient
{
    public static Client Instance;
    public bool StartupError = false;
    public Process GameProcess;
    public IntPtr GameHandle = IntPtr.Zero;

    private string _address;
    private int _port;
    private bool _stop;

    public Client(string aAddress, int aPort) : base(aAddress, aPort)
    {
        _address = aAddress;
        _port = aPort;
        Instance = this;
        GameProcess = new Process();
        GameProcess.StartInfo.UseShellExecute = false;

        string gamePath = $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\SCP Unity.exe";

        Console.WriteLine(gamePath);

        if (File.Exists(gamePath))
        {
            Console.WriteLine("Found game exe, starting process");
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

    ~Client()
    {
        if (!GameProcess.HasExited)
        {
            GameProcess.Close();
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
        Console.WriteLine($"Client connected");
        // Start receive datagrams
        ReceiveAsync();
    }

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Client disconnected");

        // Wait for a while...
        Thread.Sleep(1000);

        // Try to connect again
        if (!_stop)
            Connect();
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
}