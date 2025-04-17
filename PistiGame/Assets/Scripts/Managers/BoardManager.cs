using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class BoardManager
{
    private static readonly System.Random Rng = new System.Random();
    public static void CreateDeck(this List<int> cardArray)
    {
        for (int i = 0; i < 52; i++)
        {
            cardArray.Add(i);
        }
        cardArray.ShuffleDeck();
    }
    private static void ShuffleDeck(this List<int> array)
    {
        int n = array.Count;
        while (n > 1)
        {
            n--;
            int k = Rng.Next(n + 1);
            (array[k], array[n]) = (array[n], array[k]);
        }
    }
    public static int[] DistributeDeck(this List<int> array)
    {
        var totalArrayCount = array.Count > 4 ? 4 : array.Count;
        var selectedIds = new int[totalArrayCount];
        for (var i = 0; i < totalArrayCount; i++)
        {
            var randomIndex = Rng.Next(array.Count);
            selectedIds[i] = array[randomIndex];
            array.Remove(array[randomIndex]);
        }
        return selectedIds;
    }
    private static bool Contains(int[] array, int value)
    {
        foreach (int item in array)
        {
            if (item == value)
            {
                return true;
            }
        }
        return false;
    }

    public static string DebugDeck(this int[] deckArray)
    {
        StringBuilder newStringBuilder = new StringBuilder();
        newStringBuilder.AppendLine($"Deck Max Count : {deckArray.Length}");
        foreach (var deckValue in deckArray)
        {
            newStringBuilder.Append($"{deckValue} + ");
        }

        return newStringBuilder.ToString();
    }
}
