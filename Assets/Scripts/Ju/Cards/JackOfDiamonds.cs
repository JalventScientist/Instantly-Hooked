using UnityEngine;

public class JackOfDiamonds : Card
{
    private Hands hands;
    private Decks decks;
    [SerializeField] private int drawAmount;

    private void Start()
    {
        hands = FindAnyObjectByType<Hands>();
        decks = FindAnyObjectByType<Decks>();
    }

    public override void ApplyCard()
    {
        if (IsPlayerCard)
        {
            for (int i = 0; i < hands.playerHand.Count; i++)
                Util.DrawFromDeckIntoHand(hands.playerHand, decks.discardDeck);
            for (int i = 0; i < drawAmount; i++)
                Util.DrawFromDeckIntoHand(decks.drawDeck, hands.playerHand);
        }
        else
        {
            for (int i = 0; i < hands.enemyHand.Count; i++)
                Util.DrawFromDeckIntoHand(hands.enemyHand, decks.discardDeck);
            for (int i = 0; i < drawAmount; i++)
                Util.DrawFromDeckIntoHand(decks.drawDeck, hands.enemyHand);
        }
    }
}
