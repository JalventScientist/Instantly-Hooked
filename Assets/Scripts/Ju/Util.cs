using System.Collections.Generic;
using UnityEngine;

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
        drawTo.Add(drawFrom[0]);
        drawFrom.RemoveAt(0);
    }
    static public void DrawFromDeckIntoHand(List<string> drawFrom, List<string> drawTo, int drawFromIndex)
    {
        drawTo.Add(drawFrom[drawFromIndex]);
        drawFrom.RemoveAt(drawFromIndex);
    }
}
