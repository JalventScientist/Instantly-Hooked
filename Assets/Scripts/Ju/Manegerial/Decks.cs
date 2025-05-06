using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Decks : Util
{
    [SerializeField] private List<string> allCards = new List<string>();
    public List<string> drawDeck = new List<string>();
    public List<string> discardDeck = new List<string>();
    private List<string>[] drawAndDiscardDeck = new List<string>[2];

    private void Start()
    {
        drawAndDiscardDeck[0] = drawDeck;
        drawAndDiscardDeck[1] = discardDeck;
        ShuffleFromDeckIntoDeck(allCards, drawDeck);
    }
}
