using System;
using System.Collections.Generic;
using System.Linq;

namespace Payloads
{
    [Serializable]
    public class RoomList
    {
        public List<Room> Rooms = new();
        public RoomList() { }
        public bool IsExists(string name)
        {
            foreach (var room in Rooms)
            {
                if (room.RoomName == name) return true;
            }
            return false;
        }
        public Room GetRoom(string name)
        {
            return Rooms.FirstOrDefault(r => r.RoomName == name);
        }
    }
    [Serializable]
    public class Room
    {
        public string RoomName;
        public string HostName;
        public int MaxPlayers;
        public List<Player> Players = new();
        public Room() { }
    }
}
