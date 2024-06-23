using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "SCMP Client";

        const string filePath = "pid.txt";

        if (!File.Exists(filePath))
        {
            Console.WriteLine("pid.txt does not exist, creating it");
            File.Create(filePath).Close();
            Console.WriteLine("pid.txt was created");
        }

        try
        {
            Console.WriteLine($"Attempting to write to {filePath.Substring(filePath.LastIndexOf('\\') + 1)}");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                Process currentProcess = Process.GetCurrentProcess();
                writer.WriteLine(currentProcess.Id);
                writer.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error writing to file: {e.Message}");
        }

        Console.WriteLine($"Finished writing to {filePath.Substring(filePath.LastIndexOf('\\') + 1)}");


        // UDP server address
        string address = "127.0.0.1";

        // UDP server port
        int port = 10293;
        //if (args.Length > 1)
        //    port = int.Parse(args[1]);
        Client client = new Client(address, port);

        if (client.StartupError)
        {
            Thread.Sleep(5000);
            return;
        }

        Console.WriteLine("Client connecting...");
        if (client.Connect())
        {
            string line = "";

            while (!client.GameProcess.HasExited)
            {
                Thread.Sleep(500);
            }

            Console.WriteLine("Client disconnecting...");
            client.DisconnectAndStop();
            Console.WriteLine("Exiting...");
            Thread.Sleep(3000);
        }
    }
}