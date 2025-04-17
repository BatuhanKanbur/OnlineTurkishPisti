using System.Linq;
using Managers;
using Structures;
using UnityEngine;

namespace Objects
{
    public class Player : IPlayer
    {
        private float _aiPlayTime;
        private void Start()
        {
            if (playerType != PlayerType.AI) return;
            Init(Constants.AINames[Random.Range(0, Constants.AINames.Length)]);
        }
        public void Init(string newPlayerName)
        {
            InitPlayer(newPlayerName);
        }
        private void Update()
        {
            if(playerType != PlayerType.AI) return;
            if(!IsReady) return;
            _aiPlayTime += Time.deltaTime;
            if (!(_aiPlayTime > Constants.AIPlayTime)) return;
            Play();
            _aiPlayTime = 0;
        }
        private void Play()
        {
            var targetCardId = GameManager.Instance.GetStackLastCard();
            foreach (var playerCard in PlayerDeck.Where(playerCard => playerCard.Key == targetCardId))
            {
                PlayCard(playerCard.Key);
                return;
            }
            PlayCard(PlayerDeck.Keys.Last());
        }
    }
}
