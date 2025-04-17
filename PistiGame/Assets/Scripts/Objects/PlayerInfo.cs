using System.Collections;
using System.Collections.Generic;
using Payloads;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    public void Init(Player playerPayload)
    {
        playerNameText.text = playerPayload.Name;
        transform.localScale = Vector3.one;
    }
}
