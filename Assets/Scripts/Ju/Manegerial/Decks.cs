using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Decks : MonoBehaviour
{
    [SerializeField] private List<string> allCards = new List<string>();
    public List<string> drawDeck = new List<string>();
    public List<string> discardDeck = new List<string>();
    public List<string>[] drawAndDiscardDeck = new List<string>[2];

    private void Awake()
    {
        drawAndDiscardDeck[0] = drawDeck;
        drawAndDiscardDeck[1] = discardDeck;
        Util.ShuffleFromDeckIntoDeck(allCards, drawDeck);
    }
}
