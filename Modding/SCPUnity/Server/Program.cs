using System.Net;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "SCMP Server";

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
}