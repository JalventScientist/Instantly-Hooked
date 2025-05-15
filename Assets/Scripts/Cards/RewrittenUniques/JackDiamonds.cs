using UnityEngine;

public class JackDiamonds : Card
{
    ReadDeck deck;

    private void Start()
    {
        WillBeAssigned = false;
        deck = FindFirstObjectByType<ReadDeck>();
    }

    public override void ApplyCard()
    {
        base.ApplyCard();
        StartCoroutine(deck.DiscardOneSide(3, IsPlayerCard, true, true)); //Discard 3, player side, lock cards, add new

    }
}
