using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StackLayout : MonoBehaviour
{
    private int _childCount;
    private void Update()
    {
        if (Time.frameCount % 30 != 0) return;
        if (_childCount == transform.childCount) return;
        _childCount = transform.childCount;
        if (_childCount < 2) return;
        var randomValue = Random.Range(-5, 5);
        var child = transform.GetChild(_childCount-1);
        var childPosition = Vector2.one * randomValue;
        child.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        child.GetComponent<RectTransform>().anchoredPosition += childPosition;
        child.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, randomValue * 5);
    }
}

