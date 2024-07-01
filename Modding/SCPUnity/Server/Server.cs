using System.Net;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;

class Server : UdpServer
{
    public struct Client
    {
        public string username;
        public int id;
        public EndPoint endpoint;
    }

    public static Server Instance;
    public IPAddress address;
    public int port;
    public List<Client> ClientList;
    public static Utilities Utils = new();

    private List<int> _idList;


    public Server(IPAddress aAddress, int aPort) : base(aAddress, aPort)
    {
        port = aPort;
        address = aAddress;
        Instance = this;
        ClientList = new List<Client>();
        _idList = new List<int>();
    }

    protected override void OnStarted()
    {
        Console.WriteLine($"Started server");

        ReceiveAsync();
    }

    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    {
        // if server received from itself, just continue receiving
        if (endpoint != Endpoint)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            int.TryParse(message[0].ToString(), out var typeInt);
            EventType type = (EventType)typeInt;
            

            switch (type)
            {
                case EventType.Join:

                    Client client = new Client();
                    char[] chars = message.ToCharArray();


                    for (uint i = 1; i < size; i++)
                    {
                        if (chars[i] == '\0')
                            break;

                        if (chars[i] == '\n' || chars[i] == '\r')
                            continue;

                        client.username += chars[i];
                    }
                    // not sure what gets slapped on the end but remove it
                    client.username = client.username.Remove(client.username.Length - 1, 1);

                    int id = GenerateId();
                    while (_idList.Contains(id))
                    {
                        id = GenerateId();
                    }
                    _idList.Add(id);

                    client.id = id;
                    client.endpoint = endpoint;
                    ClientList.Add(client);
                    Utils.WriteToFile(Utils.LobbyInfoFilePath, client.username);
                    EventColors.FormatEventTypeOutput(type);
                    Console.WriteLine($"{client.username} joined with id {client.id}");

                    foreach (Client c in ClientList)
                    {
                        if(c.id != client.id)
                            Send(c.endpoint, EventType.Join, client.username);
                    }
                    break;
                case EventType.Leave:
                    break;
                case EventType.UpdateRotation:
                    break;
                case EventType.UpdateLocation:
                    break;
                case EventType.StartGame:
                    break;
                case EventType.EndGame:
                    break;
                case EventType.Acknowledge:

                    //Utils.WaitForFile(Utils.LobbyInfoFilePath, false, FileAccess.Read, FileShare.Write);
                    string lobbyInfo = Utils.ReadFileBytes(Utils.LobbyInfoFilePath);

                    if (string.IsNullOrEmpty(lobbyInfo))
                    {
                        string temp = "";

                        foreach (Client c in ClientList)
                        {
                            temp += $"{c.username},\n";
                        }

                        Utils.WriteToFile(Utils.LobbyInfoFilePath, temp);
                        lobbyInfo = Utils.ReadFileBytes(Utils.LobbyInfoFilePath);
                    }

                    // SPLIT THE STRING WITH A COMMA DELIMITER
                    string[] playerNames = lobbyInfo.Split(',');
                    char[] badChars = { '\r', '\n', ','};
                    lobbyInfo = $"{playerNames.Length}\0";
                    foreach (string name in playerNames)
                    {
                        string temp = name;
                        foreach (char c in badChars)
                        {
                            while (temp.Contains(c))
                            {
                                temp = temp.Remove(temp.IndexOf(c), 1);
                            }
                        }

                        if(temp != "")
                            lobbyInfo += $"{temp},";
                    }

                    lobbyInfo = lobbyInfo.Remove(lobbyInfo.Length - 1, 1);
                    // don't send trailing comma
                    Console.WriteLine(lobbyInfo);

                    Send(endpoint, EventType.Acknowledge); // ACK for client to know it connected in OnConnected()
                    Send(endpoint, EventType.Acknowledge, lobbyInfo);
                    break;
                case EventType.KickPlayer:
                case EventType.MessageSent:
                case EventType.MessageReceived:
                default: break;
            }

            //Console.WriteLine($"Incoming: {Encoding.UTF8.GetString(buffer, (int)offset, (int)size)}");

            //foreach (Client client in ClientList)
            //{
            //    if (endpoint != client.endpoint)
            //    {
            //        SendAsync(client.endpoint, buffer, offset, size);
            //    }
            //}
        }

        ReceiveAsync();
    }

    protected override void OnSent(EndPoint endpoint, long sent)
    {

        ReceiveAsync();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Server caught an error with code {error}");
    }

    private int GenerateId()
    {
        return new Random().Next(0, int.MaxValue / 25);
    }

    private long Send(EndPoint endpoint, EventType eventType, string value = "")
    {
        return Send(endpoint, $"{(int)eventType}{value}");
    }

    private long Send(EndPoint endpoint, EventType eventType, byte[] value)
    {
        return Send(endpoint, $"{(int)eventType}{value}");
    }
}
