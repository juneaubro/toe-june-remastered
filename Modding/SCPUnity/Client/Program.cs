using System.Diagnostics;

class Program
{

    static void Main(string[] args)
    {
        Console.Title = "SCMP Client";

        bool gameStartedFirst = false;
        if (args.Length > 0)
        {
            foreach (string arg in args)
            {
                if (arg == "-gameStarted")
                    gameStartedFirst = true;
            }
        }

        WritePID(gameStartedFirst);

        WaitForLobby();

        string address = "127.0.0.1";
        int port = 10293;

        Client client = new Client(address, port);

        if (client.StartupError)
        {
            Thread.Sleep(500);
            return;
        }

        Console.WriteLine("Client connecting...");
        if (client.Connect())
        {
            //string line = "";

            while (!client.GameProcess.HasExited)
            {
                Thread.Sleep(500);
            }

            Console.WriteLine("Client disconnecting...");
            client.DisconnectAndStop();
            Console.WriteLine("Exiting...");
            Thread.Sleep(1000);
        }
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

    public static void WritePID(bool gameStartedFirst)
    {
        string path = "pid.txt";

        // if game opened first, directory is different
        if (gameStartedFirst)
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

    public static void WaitForLobby()
    {
        
    }
}