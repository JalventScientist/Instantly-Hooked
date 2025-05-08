using UnityEngine;
using System.Collections;

public class AceDiamonds : Card
{
    ReadDeck deck;

    void Start()
    {
        deck = FindFirstObjectByType<ReadDeck>();
    }

    public override void ApplyCard()
    {
        base.ApplyCard();
        StartCoroutine(deck.RenewDecks());
    }
}
