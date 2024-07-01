using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

class Program
{
    public static Utilities Utils = new();
    private static bool _gameStartedFirst = false;
    private static bool _gameRunning = false;

    static void Main(string[] args)
    {
        Console.Title = "SCMP Server";

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

        Server server;
        bool started = false;

        try
        {
            server = new(IPAddress.Any, 10293);
            started = server.Start();
        }
        catch (SocketException e)
        {
            Console.WriteLine("Server already started. Exiting...");
            Thread.Sleep(1000);
            return;
        }


        if (started)
        {
            WritePid();

            for (;;)
            {

                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                    continue;

                if (line == "/quit")
                    break;

                if (line == "/list")
                {
                    string playerList = "";
                    foreach (Server.Client client in server.ClientList)
                    {
                        playerList += $"{client.username}, ";
                    }

                    playerList = playerList.Remove(playerList.Length - 1);
                    Console.WriteLine($"Connected clients: {playerList}");
                }

                foreach (Server.Client client in server.ClientList)
                {
                    if(client.endpoint != server.Endpoint)
                        server.Send(client.endpoint, line);
                }
            }

            Console.WriteLine("Stopping server...");
            server.Stop();
        }
    }

    public static void WritePid()
    {
        if (!Directory.Exists(Utils.BinPath))
            Directory.CreateDirectory(Utils.BinPath);

        if (File.Exists(Utils.ServerPidPath))
        {
            File.Delete(Utils.ServerPidPath);
        }

        File.Create(Utils.ServerPidPath).Close();
        Utils.WriteToFile(Utils.ServerPidPath, Process.GetCurrentProcess().Id);
    }
}