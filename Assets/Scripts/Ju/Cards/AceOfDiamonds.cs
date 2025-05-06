using UnityEngine;

public class AceOfDiamonds : Card
{
    private Decks decks;

    private void Start()
    {
        decks = FindAnyObjectByType<Decks>();
    }
    public override void ApplyCard()
    {
        Util.ShuffleFromDeckIntoDeck(decks.drawDeck, decks.drawDeck);
    }
}
