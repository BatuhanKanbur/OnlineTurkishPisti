using LiteNetLib;
namespace PistiDedicatedServer.Structures;
[Serializable]
public class RoomList
{
    public List<Room> Rooms = new();
    public RoomList() { }
}
[Serializable]
public class Room
{
    public string RoomName { get; set; }
    public string HostName { get; set; }
    public int MaxPlayers { get; set; }
    public List<Player> Players = [];
    public Room() { }
    public void AddPlayer(Player player)
    {
        if (Players.Count >= MaxPlayers) return;
        Players.Add(player);
    }
    public void RemovePlayer(string name)
    {
        var player = Players.FirstOrDefault(p => p.Name == name);
        if (player == null) return;
        Players.Remove(player);
    }
}
