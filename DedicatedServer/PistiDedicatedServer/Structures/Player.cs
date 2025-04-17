using System.Text.Json.Serialization;
using LiteNetLib;

namespace PistiDedicatedServer.Structures;

[Serializable]
public class PlayerList
{
    public List<Player> Players = new();
    public PlayerList() { }
}
[Serializable]
public class Player
{
    public string Name { get; set; }
    public string CurrentRoom { get; set; }
    public Player() { }
}
