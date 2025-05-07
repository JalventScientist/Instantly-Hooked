using UnityEngine;

public class QueenOfDiamonds : Card
{
    private Hands hands;
    private Decks decks;
    [SerializeField] private int discardAmount;

    private void Start()
    {
        hands = FindAnyObjectByType<Hands>();
        decks = FindAnyObjectByType<Decks>();
    }

    public override void ApplyCard()
    {
        if (IsPlayerCard)
            for (int i = 0; i < discardAmount; i++)
                Util.DrawFromDeckIntoHand(hands.enemyHand, decks.discardDeck, Random.Range(0, hands.enemyHand.Count));
        else
            for (int i = 0; i < discardAmount; i++)
                Util.DrawFromDeckIntoHand(hands.playerHand, decks.discardDeck, Random.Range(0, hands.playerHand.Count));
    }
}
