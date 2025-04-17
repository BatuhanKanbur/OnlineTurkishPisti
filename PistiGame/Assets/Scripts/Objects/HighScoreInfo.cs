using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreInfo : MonoBehaviour
{
    [SerializeField] private Text playerNameText,playerScoreText;
    public void Init(string playerName,int playerScore)
    {
        playerNameText.text = playerName;
        playerScoreText.text = playerScore.ToString();
        transform.localScale = Vector3.one;
    }
}
