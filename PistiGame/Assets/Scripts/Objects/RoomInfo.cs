using System;
using Payloads;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] private Text roomNameText;
    [SerializeField] private Text playerCountText;
    [SerializeField] private Button joinButton;
    private Room _roomPayload;
    private Action<Room> _onJoinButtonPressed;
    public void Init(Room roomPayload, Action<Room> onJoinButtonPressed)
    {
        _roomPayload = roomPayload;
        _onJoinButtonPressed = onJoinButtonPressed;
        roomNameText.text = roomPayload.RoomName;
        playerCountText.text = $"{roomPayload.Players.Count}/{roomPayload.MaxPlayers}";
        joinButton.gameObject.SetActive(roomPayload.Players.Count < roomPayload.MaxPlayers);
        transform.localScale = Vector3.one;
    }

    public void OnJoinButtonPressed() => _onJoinButtonPressed?.Invoke(_roomPayload);
}
