using System;
using System.Collections.Generic;
using Structures;
using UnityEngine;
using UnityEngine.UI;

namespace Objects
{
    public abstract class IPlayer : MonoBehaviour
    {
        public event Action<IPlayer,int> OnPlayCard;
        public event Action<IPlayer> OnHasNoCardsLeft;
        protected readonly Dictionary<int,Card> PlayerDeck = new Dictionary<int, Card>();
        private int _playerScore;
        private int _totalCollectedCards;
        public PlayerType playerType;
        protected bool IsReady;
        public DeckLayout deckParent;
        [SerializeField] private Text playerNameText,playerScoreText;
        public RectTransform playerTransform;
        protected void InitPlayer(string newPlayerName)
        {
            playerNameText.text = newPlayerName;
        }

        public string GetPlayerName() => playerNameText.text;

        public void SetTurn(bool isReady)
        {
            IsReady = isReady;
        }

        public void SetDecks(int[] newDeck,Card[] newCards)
        {
            for (int i = 0; i < newDeck.Length; i++)
            {
                PlayerDeck.Add(newDeck[i],newCards[i]);
                newCards[i].transform.SetParent(deckParent.transform);
                if(playerType != PlayerType.MainPlayer) continue;
                newCards[i].OnObjectClickEvent += PlayCard;
            }
            newDeck.DebugPlayerDeck(GetPlayerName());
        }

        public void AddScore(int score)
        {
            _playerScore += score;
            playerScoreText.text = _playerScore.ToString();
        }

        public void AddCollectScore(int cardCount)
        {
            _totalCollectedCards += cardCount;
        }

        public Tuple<IPlayer,int, int> GetScore() => new Tuple<IPlayer,int, int>(this,_playerScore, _totalCollectedCards);

        protected void PlayCard(int cardId)
        {
            if (IsReady)
            {
                OnPlayCard?.Invoke(this, cardId);
                PlayerDeck[cardId].OnObjectClickEvent -= PlayCard;
                PlayerDeck[cardId].gameObject.SetActive(false);
                // PlayerDeck[cardId].transform.SetParent(deckParent.transform);
                PlayerDeck.Remove(cardId);
                Debug.Log(GetPlayerName() + $" has been played the {cardId}");
                if(PlayerDeck.Count==0)
                    NoCardsLeft();
            }
            else
            {
                Debug.Log(GetPlayerName() + $" tried to play when it was not their turn");
            }
        }

        private void NoCardsLeft()
        {
            Debug.Log(GetPlayerName() + " the cards are out!");
            OnHasNoCardsLeft?.Invoke(this);
        }
    }
    public enum PlayerType
    {
        MainPlayer,
        Player,
        AI
    }
}