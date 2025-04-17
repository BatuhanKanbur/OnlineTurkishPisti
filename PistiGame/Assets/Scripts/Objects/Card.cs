using System;
using UnityEngine;
using UnityEngine.UI;

namespace Objects
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Image spriteRenderer;
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform rectTransform;
        private int _cardId;
        public event Action<int> OnObjectClickEvent;
        public void SetCard(Sprite targetSprite,int order,int newCardId)
        {
            spriteRenderer.sprite = targetSprite;
            gameObject.name = targetSprite.name;
            canvas.sortingOrder = order;
            _cardId = newCardId;
        }

        public void OnSelect()
        {
            OnObjectClickEvent?.Invoke(_cardId);
        }

        private void OnDisable()
        {
            canvas.sortingOrder =-1;
            StopAllCoroutines();
        }
    }
}
