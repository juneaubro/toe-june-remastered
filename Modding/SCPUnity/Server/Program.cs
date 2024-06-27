using System.Diagnostics;
using System.Net;

class Program
{
    private static string _binDirectory;
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
                    _binDirectory = $@"{Directory.GetCurrentDirectory()}\BepInEx\plugins\SCMP\bin";
                }
            }
        }

        if (string.IsNullOrEmpty(_binDirectory))
        {
            _binDirectory = $@"{Directory.GetCurrentDirectory()}\bin";
        }

        WritePID();


        Server server = new(IPAddress.Any, 10293);

        if (server.Start())
        {
            for (;;)
            {
                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                    continue;

                if (line == "/quit")
                    break;

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

    public static void WritePID()
    {
        string path = $@"{_binDirectory}\serverpid.txt";

        // if game opened first, directory is different
        if (_gameStartedFirst)
        {
            path = $@"{_binDirectory}\serverpid.txt";
        }

        if (!Directory.Exists(_binDirectory))
            Directory.CreateDirectory(_binDirectory);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.Create(path).Close();
        Utilities.WriteToFile(path, Process.GetCurrentProcess().Id);
    }
}