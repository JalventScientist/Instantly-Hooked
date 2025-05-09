using UnityEngine;

public class KingDiamonds : Card
{
    ReadDeck readDeck;

    private void Start()
    {
        WillBeAssigned = false;
        readDeck = FindFirstObjectByType<ReadDeck>();
    }

    public override void ApplyCard()
    {
        base.ApplyCard();
        StartCoroutine(readDeck.PullOneSide(1, IsPlayerCard, true));
    }
}
