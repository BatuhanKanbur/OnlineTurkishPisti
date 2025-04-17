using System.Text.Json;
using Newtonsoft.Json;
using PistiDedicatedServer.Structures;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace PistiDedicatedServer;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Net;
public class LobbyServer : INetEventListener
{
    private NetManager _server;
    private NetDataWriter _writer;

    private readonly Dictionary<NetPeer, Player> _players = new();
    private readonly RoomList _roomList = new();
    private readonly PlayerList _playerList = new();

    public void Start()
    {
        _server = new NetManager(this);
        _server.Start(9050);
        _writer = new NetDataWriter();
        Console.WriteLine("Server started...");
    }

    private void AddPlayer(NetPeer peer, Player player)
    {
        _players.Add(peer, player);
        _playerList.Players.Add(player);
        BroadcastPlayerList();
    }
    private void RemovePlayer(NetPeer peer){
        RemoveRoom(_players[peer].Name);
        _players.Remove(peer);
        _playerList.Players = _players.Values.ToList();
        BroadcastPlayerList();
    }
    private void AddRoom(Room room,NetPeer peer)
    {
        _players[peer].CurrentRoom = room.RoomName;
        _roomList.Rooms.Add(room);
        SendMessage(peer, ServerMessageType.RoomCreated, room);
        BroadcastRoomList();
    }
    private void JoinRoom(Room room,NetPeer peer)
    {
        var targetRoom = _roomList.Rooms.Find(r => r.RoomName == room.RoomName);
        if (targetRoom == null) return;
        var player = _players[peer];
        if (targetRoom.Players.Exists(p => p.Name == player.Name)) return;
        if (targetRoom.Players.Count >= targetRoom.MaxPlayers) return;
        player.CurrentRoom = room.RoomName;
        targetRoom.AddPlayer(player);
        _playerList.Players = _players.Values.ToList();
        SendMessage(peer, ServerMessageType.JoinedRoom, room);
        BroadcastRoomList();
    }
    private void RemoveRoom(string ownerName)
    {
        var room = _roomList.Rooms.Find(r => r.HostName == ownerName);
        if (room == null) return;
        _roomList.Rooms.Remove(room);
        BroadcastRoomList();
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
        RemovePlayer(peer);
    }
    public void OnNetworkError(IPEndPoint endPoint, System.Net.Sockets.SocketError socketError) { }
    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            var type = (ClientMessageType)reader.GetInt();
            var json = reader.GetString();
            switch (type)
            {
                case ClientMessageType.JoinLobby:
                    var joinData = Deserialize<Player>(json);
                    if(joinData==null)return;
                    var newPlayer = new Player { Name = joinData.Name };
                    AddPlayer(peer, newPlayer);
                    Console.WriteLine($"{joinData.Name} joined lobby.");
                    BroadcastMessage(ServerMessageType.PlayerJoined, joinData);
                    BroadcastRoomList();
                    break;
                case ClientMessageType.CreateRoom:
                    var createRoom = Deserialize<Room>(json);
                    if(createRoom==null)return;
                    var findRoom = _roomList.Rooms.Find(r => r.RoomName == createRoom.RoomName);
                    if (findRoom!=null)return;
                    createRoom.AddPlayer(_players[peer]);
                    AddRoom(createRoom, peer);
                    Console.WriteLine($"Room created: {createRoom.RoomName}");
                    break;

                case ClientMessageType.JoinRoom:
                    var joinRoom = Deserialize<Room>(json);
                    if(joinRoom==null)return;
                    var targetRoom = _roomList.Rooms.Find(r => r.RoomName == joinRoom.RoomName);
                    if(targetRoom==null)return;
                    JoinRoom(targetRoom,peer);
                    break;
                case ClientMessageType.ListRooms:
                    BroadcastRoomList();
                    break;
                case ClientMessageType.ListPlayers:
                    BroadcastPlayerList();
                    break;
            }

            reader.Recycle();
        }

    private void BroadcastRoomList()
    {
        BroadcastMessage(ServerMessageType.RoomList, _roomList);
    }
    private void BroadcastPlayerList()
    {
        BroadcastMessage(ServerMessageType.PlayerList, _playerList);
    }

    private void BroadcastMessage<T>(ServerMessageType type, T payload)
    {
        foreach (var player in _players)
            SendMessage(player.Key, type, payload);
    }

    private void SendMessage<T>(NetPeer peer, ServerMessageType type, T payload)
    {
        _writer.Reset();
        _writer.Put((int)type);
        _writer.Put(JsonConvert.SerializeObject(payload));
        peer.Send(_writer, DeliveryMethod.ReliableOrdered);
    }

    private T? Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}

