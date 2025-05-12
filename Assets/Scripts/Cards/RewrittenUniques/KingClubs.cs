using UnityEngine;

public class KingClubs : Card
{
    ReadDeck readDeck;
    private void Start()
    {
        WillBeAssigned = false;
        readDeck = FindFirstObjectByType<ReadDeck>();
    }

    public override void ApplyCard()
    {
        if (IsPlayerCard)
        {
            readDeck.MaxPlayerDeck++;
        } else
        {
            readDeck.MaxEnemyDeck++;
        }
        StartCoroutine(readDeck.PullOneSide(1, IsPlayerCard, true));
        base.ApplyCard();
    }
}
