using System;
using System.Collections.Generic;
using LiteNetLib;

namespace Payloads
{
    [Serializable]
    public class PlayerList
    {
        public List<Player> Players = new();
        public PlayerList() { }
        public bool IsExists(string name)
        {
            foreach (var player in Players)
            {
                if (player.Name == name) return true;
            }
            return false;
        }
    }
    [Serializable]
    public class Player
    {
        public string Name;
        public string CurrentRoom;
        public Player() { }
    }
}
