using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class AceOfDiamonds : Card
{
    private Decks decks;
    private Hands hands;

    private void Start()
    {
        decks = FindAnyObjectByType<Decks>();
        hands = FindAnyObjectByType<Hands>();
    }
    public override void ApplyCard()
    {
        Util.ShuffleFromDeckIntoDeck(decks.drawDeck, decks.drawDeck);
        if (IsPlayerCard)
            while (hands.playerHand.Count > 0)
                Util.DrawFromDeckIntoHand(hands.playerHand, decks.discardDeck);
        else
            while (hands.enemyHand.Count > 0)
                Util.DrawFromDeckIntoHand(hands.enemyHand, decks.discardDeck);
    }
}
