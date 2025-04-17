using PistiDedicatedServer;

class Program
{
    private static void Main(string[] args)
    {
        var server = new LobbyServer();
        server.Start();

        while (true)
        {
            server.PollEvents();
            Thread.Sleep(15);
        }
    }
}
