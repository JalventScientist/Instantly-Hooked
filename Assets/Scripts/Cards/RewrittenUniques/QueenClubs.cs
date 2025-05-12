using UnityEngine;

public class QueenClubs : Card
{
    ReadDeck readDeck;
    private void Start()
    {
        WillBeAssigned = false;
        readDeck = FindFirstObjectByType<ReadDeck>();
    }

    public override void ApplyCard()
    {
        if (!IsPlayerCard)
        {
            readDeck.MaxPlayerDeck--;
        }
        else
        {
            readDeck.MaxEnemyDeck--;
        }
        StartCoroutine(readDeck.DiscardOneSide(1, !IsPlayerCard));
        base.ApplyCard();
    }
}
