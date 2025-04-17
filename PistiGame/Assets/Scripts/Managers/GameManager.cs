using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enums;
using Objects;
using Payloads;
using Structures;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Player = Objects.Player;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private AssetReference cardPrefab;
        #region Menu
        [SerializeField] private Player mainPlayer;
        public Action<List<Tuple<IPlayer,int,int>>> OnGameFinish;
        private void FinishGame()
        {
            var playerScores = new List<Tuple<IPlayer,int, int>>();
            foreach (var player in _playerDictionary)
                playerScores.Add(new Tuple<IPlayer, int, int>(player.Key.GetScore().Item1,player.Key.GetScore().Item2,player.Key.GetScore().Item3));
            playerScores.Sort((a, b) => b.Item3.CompareTo(a.Item3));
            playerScores[0] = new Tuple<IPlayer, int, int>(playerScores[0].Item1, playerScores[0].Item2 + 3,playerScores[0].Item3);
            OnGameFinish?.Invoke(playerScores);
        }
        #endregion
        #region GamePlayFields
        private readonly List<int> _deckArray = new List<int>();
        private readonly Dictionary<int,Card> _stackArray = new Dictionary<int, Card>();
        private int _playerTurn;
        public IPlayer[] players;
        [SerializeField] private Text deckCountText;
        [SerializeField] private RectTransform tableDeck, tableStack;
        private readonly Dictionary<IPlayer, int> _playerDictionary = new Dictionary<IPlayer, int>();
        private int PlayerTurn
        {
            get => _playerTurn;
            set
            {
                if (value > _playerDictionary.Count - 1)
                    PlayerTurn = 0;
                if (_playerTurn != value)
                    PlayerTurnChanged(value);
                _playerTurn = value;
            }
        }
        #endregion

        private GameAssetManager _gameAssetManager;
        public async void StartGame(Room roomPayload,string playerName)
        {            
            mainPlayer.Init(playerName);
            _gameAssetManager = GameAssetManager.Instance;
            await _gameAssetManager.LoadAssets();
            _deckArray.CreateDeck();
            for (var i = 0; i < roomPayload.MaxPlayers; i++)
            {
                players[i].gameObject.SetActive(true);
                players[i].OnPlayCard += Play;
                players[i].OnHasNoCardsLeft += NoCardsLeft;
                _playerDictionary.Add(players[i],i);
            }
            var newStackArray = _deckArray.DistributeDeck();
            for (var i = 0; i < newStackArray.Length; i++)
            {
                Debug.Log(newStackArray[i]);
                var cardSprite = i < newStackArray.Length - 1 ? _gameAssetManager.GetCardBackFace() : _gameAssetManager.GetCard(newStackArray[i]);
                var newCardObject = await ObjectManager.GetObject(cardPrefab);
                newCardObject.transform.SetParent(tableStack);
                var newCard = newCardObject.GetComponent<Card>();
                newCard.SetCard(cardSprite,_stackArray.Count,newStackArray[i]);
                _stackArray.Add(newStackArray[i],newCard);
            }

            foreach (var player in _playerDictionary)
            {
                player.Key.SetTurn(player.Value == PlayerTurn);
                DistributeDeckToPlayer(player.Key);
            }
            
        }
        private void PlayerTurnChanged(int turnId)
        {
            foreach (var playerDict in _playerDictionary)
            {
                playerDict.Key.SetTurn(playerDict.Value == turnId);
            }
        }
        public int GetStackLastCard() => _stackArray.Count==0 ? -1 : _stackArray.Keys.Last();
        private async void DistributeDeckToPlayer(IPlayer targetPlayer)
        {
            if (_deckArray.Count < 1)
            {
                FinishGame();
                return;
            }
            var newDeck = _deckArray.DistributeDeck();
            var newCardList = new List<Card>();
            foreach (var cardId in newDeck)
            {
                Sprite cardSprite = targetPlayer.playerType != PlayerType.MainPlayer ? _gameAssetManager.GetCardBackFace() : _gameAssetManager.GetCard(cardId);
                var newCardObject =await ObjectManager.GetObject(cardPrefab);
                Card newCard = newCardObject.GetComponent<Card>();
                newCard.transform.SetParent(tableStack.transform);
                newCard.transform.SetPositionAndRotation(tableStack.position, tableStack.rotation);
                newCard.transform.Move(targetPlayer.transform,0.25f);
                newCard.SetCard(cardSprite,0,cardId);
                newCardList.Add(newCard);
            }
            await Task.Delay(250);
            targetPlayer.SetDecks(newDeck,newCardList.ToArray());
            deckCountText.text = _deckArray.Count.ToString();
        }
        private async void Play(IPlayer player, int cardId)
        {
            if (PlayerTurn == _playerDictionary[player])
            {
                var newCardObject = await ObjectManager.GetObject(cardPrefab);
                Card newCard = newCardObject.GetComponent<Card>();
                newCard.SetCard(_gameAssetManager.GetCard(cardId),_stackArray.Count,cardId);
                newCard.transform.SetParent(tableDeck);
                newCard.transform.SetPositionAndRotation(player.deckParent.transform.position,player.deckParent.transform.rotation);
                newCard.transform.SetAnchoredPositionAndRotation(tableDeck.anchoredPosition,new Vector3(0,0,Random.Range(-30,30)),0.5f,OnCardGoTarget);
                void OnCardGoTarget()
                {
                    if (CardIsMatch(cardId, out var takeType))
                    {
                        switch (takeType)
                        {
                            case TakeType.Normal:
                                player.AddScore(Constants.NormalScore);
                                break;
                            case TakeType.Club2:
                                player.AddScore(Constants.Club2Score);
                                break;
                            case TakeType.Diamond10:
                                player.AddScore(Constants.Diamond10Score);
                                break;
                            case TakeType.AsJoker:
                                player.AddScore(Constants.AsJokerScore);
                                break;
                            case TakeType.Joker:
                                player.AddScore(Constants.JokerScore);
                                break;
                        }
                       _stackArray.Add(cardId,newCard);
                        player.AddCollectScore(_stackArray.Count);
                        foreach (var stack in _stackArray)
                        {
                            var cardObject = stack.Value.gameObject;
                            cardObject.transform.SetParent(player.transform);
                            var deckTransform = player.GetComponent<RectTransform>().anchoredPosition;
                            cardObject.transform.MoveAnchored(deckTransform,1f, ClearStack);
                        }
                        Debug.Log($"{player.GetPlayerName()} took the cards on the table via {takeType.ToString()}");
                    }
                    else
                    {
                        _stackArray.Add(cardId,newCard);
                    }
                }
                
                if (PlayerTurn + 1 > _playerDictionary.Count - 1)
                    PlayerTurn = 0;
                else
                    PlayerTurn++;
            }
        }
        private void ClearStack()
        {
            foreach (var stack in _stackArray)
            {
                stack.Value.gameObject.SetActive(false);
            }
            _stackArray.Clear();
        }
        private bool CardIsMatch(int cardId,out TakeType takeType)
        {
            if (GetStackLastCard() < 0)
            {
                takeType = TakeType.Normal;
                return false;
            }

            int stackCardId = GetStackLastCard() % 13;
            int playerCardId = cardId % 13;
            if (stackCardId == playerCardId)
            {
                if (stackCardId == Constants.JokerId && playerCardId == Constants.JokerId)
                {
                    takeType = TakeType.Joker;
                }
                else if(cardId==Constants.Diamond10Id)
                {
                    takeType = TakeType.Diamond10;
                }
                else if(cardId==Constants.Club2Id)
                {
                    takeType = TakeType.Club2;
                }
                else
                {
                    takeType = TakeType.Normal;
                }
                return true;
            }

            if(playerCardId == Constants.JokerId | playerCardId == Constants.AsId)
            {
                takeType = TakeType.AsJoker;
                return true;
            }

            takeType = TakeType.Normal;
            return false;
        }
        private void NoCardsLeft(IPlayer player) =>  DistributeDeckToPlayer(player);
        
    }
}
