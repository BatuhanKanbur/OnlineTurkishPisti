using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Structures;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameAssetManager : Singleton<GameAssetManager>
{
    public AssetReference playingCardsAssetReference;
    private IList<Sprite> _cardsSprites;
    
    public async UniTask LoadAssets()
    {
        _cardsSprites = await AssetManager<Sprite>.LoadList(playingCardsAssetReference);
    }

    public Sprite GetCard(int cardId) => _cardsSprites[cardId];
    public Sprite GetCardBackFace() => _cardsSprites[^1];
}
