namespace PistiDedicatedServer;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Net;
public class Player
{
    public NetPeer Peer;
    public string Name;
    public string CurrentRoom = null;
}

public class Room
{
    public string RoomName;
    public List<Player> Players = new();
}

public class LobbyServer : INetEventListener
{
    private NetManager _server;
    private NetDataWriter _writer;

    private readonly Dictionary<NetPeer, Player> _players = new();
    private readonly Dictionary<string, Room> _rooms = new();

    public void Start()
    {
        _server = new NetManager(this);
        _server.Start(9050);
        _writer = new NetDataWriter();
        Console.WriteLine("Server started...");
    }

    public void PollEvents() => _server.PollEvents();

    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey("MyGameKey");
    }
    public void OnPeerConnected(NetPeer peer)
    {
        Console.WriteLine($"Client connected: {peer.Address}");
    }
    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Console.WriteLine($"Client disconnected: {peer.Address}");
        if (_players.TryGetValue(peer, out var player))
        {
            BroadcastToAll($"LEFT_LOBBY|{player.Name}");
            _players.Remove(peer);
        }
    }
    public void OnNetworkError(IPEndPoint endPoint, System.Net.Sockets.SocketError socketError) { }
    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var msg = reader.GetString();
        reader.Recycle();

        var parts = msg.Split('|');
        var cmd = parts[0];

        switch (cmd)
        {
            case "JOIN_LOBBY":
                var name = parts[1];
                var newPlayer = new Player { Peer = peer, Name = name };
                _players[peer] = newPlayer;
                Console.WriteLine($"{name} joined lobby.");
                BroadcastToAll($"JOINED_LOBBY|{name}");
                break;

            case "CREATE_ROOM":
                var roomName = parts[1];
                if (!_rooms.ContainsKey(roomName))
                {
                    var room = new Room { RoomName = roomName };
                    room.Players.Add(_players[peer]);
                    _rooms[roomName] = room;
                    _players[peer].CurrentRoom = roomName;
                    Send(peer, $"ROOM_CREATED|{roomName}");
                    Console.WriteLine($"{_players[peer].Name} created room {roomName}");
                }
                break;

            case "JOIN_ROOM":
                var joinRoom = parts[1];
                if (_rooms.TryGetValue(joinRoom, out var existingRoom))
                {
                    existingRoom.Players.Add(_players[peer]);
                    _players[peer].CurrentRoom = joinRoom;
                    Send(peer, $"JOINED_ROOM|{joinRoom}");
                    Console.WriteLine($"{_players[peer].Name} joined room {joinRoom}");
                }
                break;

            case "LIST_ROOMS":
                foreach (var r in _rooms.Keys)
                    Send(peer, $"ROOM|{r}");
                break;

            case "LIST_PLAYERS":
                foreach (var p in _players.Values)
                    Send(peer, $"PLAYER|{p.Name}");
                break;
        }
    }
    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
    private void Send(NetPeer peer, string msg)
    {
        _writer.Reset();
        _writer.Put(msg);
        peer.Send(_writer, DeliveryMethod.ReliableOrdered);
    }
    private void BroadcastToAll(string msg)
    {
        foreach (var p in _players.Values)
            Send(p.Peer, msg);
    }
}

