using UnityEngine;

public class KingOfDiamonds : Card
{
    private Hands hands;
    private Decks decks;
    public int drawAmount;

    private void Start()
    {
        hands = FindAnyObjectByType<Hands>();
        decks = FindAnyObjectByType<Decks>();
    }
    public override void ApplyCard()
    {
        base.ApplyCard();
        if (IsPlayerCard)
            for (int i = 0; i < drawAmount; i++)
                Util.DrawFromDeckIntoHand(decks.drawDeck, hands.playerHand);
        else
            for (int i = 0; i < drawAmount; i++)
                Util.DrawFromDeckIntoHand(decks.drawDeck, hands.enemyHand);
    }
}
