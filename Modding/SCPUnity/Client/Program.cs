class Program
{

    static void Main(string[] args)
    {

        bool disconnect = false;

        // UDP server address
        string address = "128.0.0.1";

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

        // Connect the client
        Console.WriteLine("Client connecting...");
        //Debug.Log("Done!");

        if (client.Connect())
        {
            string line = "";
            while (!disconnect)
            {
                line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                    break;

                if (line == "/quit")
                {
                    break;
                }

            }

            Console.WriteLine("Client disconnecting...");
            client.DisconnectAndStop();
        }
    }
}