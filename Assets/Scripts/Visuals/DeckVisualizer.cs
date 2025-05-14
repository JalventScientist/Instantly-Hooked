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
        
        float DiscardPercentage = InitialOffset * 1+(deck.discardDeck.Count/52);
        float DrawPercentage = InitialOffset * 1+(deck.drawDeck.Count / 52);
        print((deck.drawDeck.Count / 52));
        print(DrawPercentage);

        Draw.DOLocalMoveY(DrawPercentage, 0.3f).SetEase(Ease.OutQuad);
        Discard.DOLocalMoveY(DiscardPercentage, 0.3f).SetEase(Ease.OutQuad);
    }
}
