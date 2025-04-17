using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Enums;
using Managers;
using Objects;
using Payloads;
using Structures;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]private NetworkClient networkClient;
    [SerializeField]private GameManager gameManager;
    [Header("Start Panel")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private InputField playerNameInputField;
    [Header("Lobby Panel")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private Transform playerListContent,roomListContent;
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private Text roomCountText;
    [SerializeField] private AssetReference playerInfoPrefab,roomInfoPrefab;
    private int _roomMaxPlayers = 2;
    private PlayerList _playerList;
    private RoomList _roomList;
    private Dictionary<string, PlayerInfo> _playerInfos = new Dictionary<string, PlayerInfo>();
    private Dictionary<string, RoomInfo> _roomInfos = new Dictionary<string, RoomInfo>();
    [Header("Game Panel")]
    [SerializeField] private GameObject gamePanel;
    [Header("Highscore Panel")]
    [SerializeField] private GameObject highScorePanel;
    private MenuState _menuState;
    public MenuState MenuState
    {
        get => _menuState;
        set
        {
            if(_menuState != value)
                OnMenuStateChanged(value);
            _menuState = value;
        }
    }

    private void OnEnable()
    {
        networkClient.OnConnected += OnConnected;
        networkClient.OnDisconnected += OnDisconnected;
        networkClient.OnJoinedRoom += OnJoinedRoom;
        networkClient.OnPlayerJoined += OnPlayerJoined;
        networkClient.OnPlayerLeft += OnPlayerLeft;
        networkClient.OnRoomCreated += OnRoomCreated;
        networkClient.OnPlayerList += OnPlayerList;
        networkClient.OnRoomList += OnRoomList;
        gameManager.OnGameFinish += OnGameFinish;
    }

    private void OnMenuStateChanged(MenuState newState)
    {
        startPanel.SetActive(newState == MenuState.Start);
        lobbyPanel.SetActive(newState == MenuState.Lobby);
        gamePanel.SetActive(newState == MenuState.Game);
        highScorePanel.SetActive(newState == MenuState.GameOver);
        switch (newState)
        {
            case MenuState.Start:
                break;
            case MenuState.Lobby:
                break;
            case MenuState.Game:
                gameManager.StartGame(GetRoomSettings(),networkClient.MainPlayerName);
                break;
            case MenuState.GameOver:
                break;
        }
    }

    private void OnDisable()
    {
        networkClient.OnConnected -= OnConnected;
        networkClient.OnDisconnected -= OnDisconnected;
        networkClient.OnJoinedRoom -= OnJoinedRoom;
        networkClient.OnPlayerJoined -= OnPlayerJoined;
        networkClient.OnPlayerLeft -= OnPlayerLeft;
        networkClient.OnRoomCreated -= OnRoomCreated;
        networkClient.OnPlayerList -= OnPlayerList;
        networkClient.OnRoomList -= OnRoomList;
        gameManager.OnGameFinish -= OnGameFinish;
    }

    #region Start State
    public void OnConnectButtonPressed()
    {
        networkClient.ConnectToServer(playerNameInputField.text);
    }
    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
    #endregion
    #region Lobby State
    public void OnCreateRoomButtonPressed()
    {
        var roomPayload = new Room {
            RoomName = roomNameInputField.text,
            HostName = playerNameInputField.text,
            MaxPlayers = _roomMaxPlayers
        };
        networkClient.CreateRoom(roomPayload);
    }

    private void OnJoinRoomButtonPressed(Room room)
    {
        networkClient.JoinRoom(room);
    }
    public void OnPlayerCountButtonPressed(int additiveCount)
    {
        _roomMaxPlayers += additiveCount;
        if (_roomMaxPlayers < 2)
            _roomMaxPlayers = 4;
        else if (_roomMaxPlayers > 4)
            _roomMaxPlayers = 2;
        roomCountText.text = _roomMaxPlayers.ToString();
    }

    public void OnDisconnectButtonPressed()
    {
        networkClient.Disconnect();
    }

    private Room GetRoomSettings()
    {
        var mainPlayerName = networkClient.MainPlayerName;
        foreach (var room in _roomList.Rooms.Where(room => room.HostName == mainPlayerName))
            return room;
        foreach (var room in _roomList.Rooms)
            if (room.Players.Any(roomPlayer => roomPlayer.Name == mainPlayerName))
                return room;
        return new Room {HostName = mainPlayerName, RoomName = "DefaultRoom", MaxPlayers = 2};
    }
    #endregion
    #region Finish State
    [SerializeField] private HighScoreInfo[] highScoreInfos;
    private void OnGameFinish(List<Tuple<IPlayer,int,int>> playerScores)
    {
        for (var i = 0; i < playerScores.Count; i++)
        {
            var player = playerScores[i];
            highScoreInfos[i].gameObject.SetActive(true);
            highScoreInfos[i].Init(player.Item1.GetPlayerName(), player.Item2);
        }
        MenuState = MenuState.GameOver;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(Constants.DemoSceneName);
    }
    #endregion
    #region NetworkManager Callbacks
    private void OnConnected()
    {
        MenuState = MenuState.Lobby;
    }

    private void OnDisconnected()
    {
        MenuState = MenuState.Start;
    }

    private void OnJoinedRoom(object obj)
    {
        MenuState = MenuState.Game;
    }

    private void OnPlayerJoined(object obj)
    {
        MenuState = MenuState.Lobby;
    }

    private void OnPlayerLeft(object obj)
    {
    }

    private void OnRoomCreated(object obj)
    {
        MenuState = MenuState.Game;
    }

    private async void OnPlayerList(object obj)
    {
        _playerList = obj as PlayerList;
        if (_playerList == null) return;
        foreach (var key in _playerInfos.Keys.Where(key => !_playerList.IsExists(key)).ToList())
        {
            _playerInfos[key].gameObject.SetActive(false);
            _playerInfos.Remove(key);
        }
        foreach (var player in _playerList.Players)
        {
            if (_playerInfos.ContainsKey(player.Name)) continue;
            var playerInfoGameObject =await ObjectManager.GetObject(playerInfoPrefab);
            var playerInfo = playerInfoGameObject.GetComponent<PlayerInfo>();
            playerInfo.transform.SetParent(playerListContent);
            playerInfo.GetComponent<PlayerInfo>().Init(player);
            _playerInfos.Add(player.Name, playerInfo);
        }
    }
    
    private async void OnRoomList(object obj)
    {
        _roomList = obj as RoomList;
        if (_roomList == null) return;
        foreach (var key in _roomInfos.Keys.ToList())
        {
            if (_roomList.IsExists(key))
            {
                _roomInfos[key].Init(_roomList.GetRoom(key),OnJoinRoomButtonPressed);
            }
            else
            {
                _roomInfos[key].gameObject.SetActive(false);
                _roomInfos.Remove(key);
            }
        }
        foreach (var room in _roomList.Rooms)
        {
            if (_roomInfos.ContainsKey(room.RoomName)) continue;
            var roomInfoGameObject = await ObjectManager.GetObject(roomInfoPrefab);
            roomInfoGameObject.transform.SetParent(roomListContent);
            var roomInfo = roomInfoGameObject.GetComponent<RoomInfo>();
            roomInfo.Init(room,OnJoinRoomButtonPressed);
            _roomInfos.Add(room.RoomName, roomInfo);
        }
        
    }

   

    #endregion
}
