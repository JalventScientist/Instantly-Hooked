using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using NUnit.Framework.Constraints;

public class Util : MonoBehaviour
{
    static public void ShuffleFromDeckIntoDeck(List<string> shuffleFrom, List<string> shuffleTo)
    {
        List<string> intermediateShuffleDeck = new List<string>();
        foreach (string card in shuffleFrom)
        {
            intermediateShuffleDeck.Add(card);
        }
        shuffleFrom.Clear();
        for (int i = 0; i < intermediateShuffleDeck.Count; i = 0)
        {
            int shufflecard = Random.Range(0, intermediateShuffleDeck.Count);
            shuffleTo.Add(intermediateShuffleDeck[shufflecard]);
            intermediateShuffleDeck.RemoveAt(shufflecard);
        }
    }

    static public void ShuffleFromDeckIntoDeck(List<string>[] shuffleFrom, List<string> shuffleTo)
    {
        List<string> intermediateShuffleDeck = new List<string>();
        foreach (List<string> cards in shuffleFrom)
        {
            foreach (string card in cards)
            {
                intermediateShuffleDeck.Add(card);
            }
            cards.Clear();
        }
        for (int i = 0; i < intermediateShuffleDeck.Count; i = 0)
        {
            int shufflecard = Random.Range(0, intermediateShuffleDeck.Count);
            shuffleTo.Add(intermediateShuffleDeck[shufflecard]);
            intermediateShuffleDeck.RemoveAt(shufflecard);
        }
    }

    static public void DrawFromDeckIntoHand(List<string> drawFrom, List<string> drawTo)
    {
        if (drawFrom.Count == 0)
            return;
        drawTo.Add(drawFrom[0]);
        drawFrom.RemoveAt(0);
    }
    static public void DrawFromDeckIntoHand(List<string> drawFrom, List<string> drawTo, int drawFromIndex)
    {
        if (drawFrom.Count == 0)
            return;
        drawTo.Add(drawFrom[drawFromIndex]);
        drawFrom.RemoveAt(drawFromIndex);
    }
    /// <summary>
    /// Returns -1 if item is not in the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="find"></param>
    /// <returns></returns>
    static public int FindIndexOfItemInArray(char[] array, char find)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == find)
            {
                return i;
            }
        }
        return -1;
    }

    static IEnumerator TypeWriteAnim(string text, TMP_Text asset, float delay)
    {
        WaitForSeconds wait = new WaitForSeconds(delay);
        for (int i = 0; i < text.Length; i++)
        {
            asset.text += text[i];
            yield return new WaitForSeconds(delay);
        }
    }

    static public void TypeWrite(MonoBehaviour runner,string text, TMP_Text asset, float delay = 0.05f)
    {
        runner.StartCoroutine(TypeWriteAnim(text, asset, delay));
    }
}
