using LiteNetLib;
using LiteNetLib.Utils;
using System;
using UnityEngine;

public class NetworkClient : MonoBehaviour, INetEventListener
{
    private NetManager _client;
    private NetDataWriter _writer;
    private NetPeer _server;

    public string playerName = "UnityPlayer";

    void Start()
    {
        _client = new NetManager(this);
        _client.Start();
        _client.Connect("localhost", 9050, "MyGameKey"); // IP ve Port sunucudakiyle aynı olmalı
        _writer = new NetDataWriter();
    }

    void Update()
    {
        _client.PollEvents();

        // Test: L tuşuna basınca JOIN_LOBBY mesajı gönder
        if (Input.GetKeyDown(KeyCode.L) && _server != null)
        {
            Send($"JOIN_LOBBY|{playerName}");
        }
    }

    public void Send(string message)
    {
        _writer.Reset();
        _writer.Put(message);
        _server.Send(_writer, DeliveryMethod.ReliableOrdered);
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("Connected to server.");
        _server = peer;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("Disconnected from server.");
    }

    public void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
    {
        Debug.LogError($"Network error: {socketError}");
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        string msg = reader.GetString();
        reader.Recycle();
        Debug.Log($"Received from server: {msg}");

        // Burada mesajlara göre işlem yapabilirsin, örneğin JOINED_LOBBY geldiyse sahne değiştirmek gibi
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey("MyGameKey");
    }

    public void OnNetworkReceiveUnconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
}
