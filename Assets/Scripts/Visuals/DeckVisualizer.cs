using UnityEngine;
using DG.Tweening;
public class DeckVisualizer : MonoBehaviour
{
    ReadDeck deck;

    [SerializeField] Transform Discard;
    [SerializeField] Transform Draw;

    private void Awake()
    {
        deck = FindFirstObjectByType<ReadDeck>();
    }
    public void UpdateDecks()
    {
        float InitialOffset = -1.07f;

        float DiscardPercentage = InitialOffset * (1 + (deck.discardDeck.Count / 52f) + (1 - Mathf.Clamp01(deck.discardDeck.Count)));
        float DrawPercentage = InitialOffset * (1 + (deck.drawDeck.Count / 52f) + (1 - Mathf.Clamp01(deck.drawDeck.Count)));
        print((deck.drawDeck.Count / 52));
        print(DrawPercentage);

        Draw.DOLocalMoveY(DiscardPercentage, 0.3f).SetEase(Ease.OutQuad);
        Discard.DOLocalMoveY(DrawPercentage, 0.3f).SetEase(Ease.OutQuad);
    }
}
