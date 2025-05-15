using UnityEngine;

public class QueenDiamond : Card
{
    ReadDeck readDeck;

    private void Start()
    {
        WillBeAssigned = false;
        readDeck = FindFirstObjectByType<ReadDeck>();
    }

    public override void ApplyCard()
    {
        StartCoroutine(readDeck.DiscardOneSide(1, !IsPlayerCard));
        base.ApplyCard();
    }
}
