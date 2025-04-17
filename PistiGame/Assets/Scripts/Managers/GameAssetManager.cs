using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Managers;
using Structures;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameAssetManager : Singleton<GameAssetManager>
{
    public AssetReference playingCardsAssetReference;
    private IList<Sprite> _cardsSprites;
    private GameObject _cardPrefab;
    
    public async Task LoadAssets()
    {
        _cardPrefab = await AssetLoader<GameObject>.LoadObject(Constants.CardPrefabName);
        _cardsSprites = await AssetLoader<Sprite>.LoadList(playingCardsAssetReference);
    }

    public Sprite GetCard(int cardId) => _cardsSprites[cardId];
    public Sprite GetCardBackFace() => _cardsSprites[^1];
    public GameObject GetCardPrefab() => _cardPrefab;
}
