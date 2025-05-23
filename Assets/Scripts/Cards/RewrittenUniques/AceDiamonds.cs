using UnityEngine;
using System.Collections;

public class AceDiamonds : Card
{
    ReadDeck deck;

    void Start()
    {
        WillBeAssigned = false;
        deck = FindFirstObjectByType<ReadDeck>();
    }

    public override void ApplyCard()
    {
        StartCoroutine(deck.RenewDecks());
        base.ApplyCard();

    }
}
