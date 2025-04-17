using UnityEngine;

public class TweenManager : MonoBehaviour
{
    public static TweenManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
