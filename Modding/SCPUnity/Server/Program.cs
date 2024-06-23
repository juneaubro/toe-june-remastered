using System.Net;

class Program
{
    static void Main(string[] args)
    {
        Server server = new(IPAddress.Loopback, 10293);

        Console.WriteLine($"Starting server : {server.Address}:{server.Port}");

        if (server.Start())
        {

            for (; ; )
            {
                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                    break;

                if (line == "/quit")
                {
                    Console.WriteLine("Stopping server");
                    break;
                }
            }

            server.Stop();
        }
    }
}