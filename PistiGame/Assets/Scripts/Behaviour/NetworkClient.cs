using LiteNetLib;
using LiteNetLib.Utils;
using System;
using Enums;
using Newtonsoft.Json;
using Payloads;
using UnityEngine;

public class NetworkClient : MonoBehaviour, INetEventListener
{
    private NetManager _client;
    private NetDataWriter _writer;
    private NetPeer _server;
    public Action OnConnected;
    public Action OnDisconnected;
    public Action<object> OnPlayerJoined;
    public Action<object> OnPlayerLeft;
    public Action<object> OnPlayerList;
    public Action<object> OnRoomCreated;
    public Action<object> OnJoinedRoom;
    public Action<object> OnRoomList;
    public string MainPlayerName{ get; private set; }
    private void Update()
    {
        _client?.PollEvents();
    }
    public void ConnectToServer(string playerName)
    {
        if (string.IsNullOrEmpty(playerName)) return;
        MainPlayerName = playerName;
        Debug.Log("Connecting to server...");
        _client = new NetManager(this);
        _writer = new NetDataWriter();
        _client.Start();
        _client.Connect("localhost", 9050, "MyGameKey");
    }

    public void CreateRoom(Room roomPayload)
    {
        SendMessage(ClientMessageType.CreateRoom,roomPayload);
    }
    public void JoinRoom(Room roomPayload)
    {
        SendMessage(ClientMessageType.JoinRoom,roomPayload);
    }

    public void Disconnect()
    {
        _client.DisconnectAll();
    }
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("Connected to server!");
        _server = peer;
        var payload = new Player { Name = MainPlayerName };
        SendMessage(ClientMessageType.JoinLobby, payload);
        OnConnected?.Invoke();
    }
    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        OnDisconnected?.Invoke();
    }
    public void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
    {
        Debug.LogError($"Network error: {socketError}");
    }
    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var type = (ServerMessageType)reader.GetInt();
        var json = reader.GetString();
        reader.Recycle();
        switch (type)
        {
            case ServerMessageType.PlayerJoined:
                var joined = JsonConvert.DeserializeObject<Player>(json);
                Debug.Log($"Player joined: {joined.Name}");
                OnPlayerJoined?.Invoke(joined);
                break;
            case ServerMessageType.PlayerLeft:
                var left = JsonConvert.DeserializeObject<Player>(json);
                Debug.Log($"Player left: {left.Name}");
                OnPlayerLeft?.Invoke(left);
                break;

            case ServerMessageType.PlayerList:
                var list = JsonConvert.DeserializeObject<PlayerList>(json);
                OnPlayerList?.Invoke(list);
                break;

            case ServerMessageType.RoomCreated:
                var roomCreated = JsonConvert.DeserializeObject<Room>(json);
                Debug.Log($"Room created: {roomCreated.RoomName}");
                OnRoomCreated?.Invoke(roomCreated);
                break;

            case ServerMessageType.JoinedRoom:
                var joinedRoom = JsonConvert.DeserializeObject<Room>(json);
                Debug.Log($"Joined room: {joinedRoom.RoomName}");
                OnJoinedRoom?.Invoke(joinedRoom);
                break;

            case ServerMessageType.RoomList:
                var room = JsonConvert.DeserializeObject<RoomList>(json);
                OnRoomList?.Invoke(room);
                break;
        }
    }
    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey("MyGameKey");
    }
    public void OnNetworkReceiveUnconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
    public void SendMessage<T>(ClientMessageType type, T payload)
    {
        if (_server == null) return;
        _writer.Reset();
        _writer.Put((int)type);
        _writer.Put(JsonUtility.ToJson(payload));
        _server.Send(_writer, DeliveryMethod.ReliableOrdered);
    }
}