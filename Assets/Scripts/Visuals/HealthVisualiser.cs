using DG.Tweening;
using UnityEngine;

public class HealthVisualiser : MonoBehaviour
{
    EvaluateDamage Damage;

    [SerializeField] Transform Plr;
    [SerializeField] Transform Ene;

    private void Awake()
    {
        Damage = FindFirstObjectByType<EvaluateDamage>();
    }
    public void UpdateDecks()
    {
        float InitialOffset = -1.55f;
        float EndOffset = -2.831f;

        float PlrPercentage = Damage.plrHealth / 50f;   
        float EnePercentage = Damage.enemyHealth / 50f;



        if (Damage.plrHealth >= 50)
        {
            PlrPercentage = 1;
        } else if (Damage.plrHealth <= 0)
            PlrPercentage = 0;
        if (Damage.enemyHealth >= 50)
        {
            EnePercentage = 1;
        }
        else if (Damage.enemyHealth <= 0)
            EnePercentage = 0;

        float PlrPos = Mathf.Lerp(EndOffset, InitialOffset, PlrPercentage);
        float EnePos = Mathf.Lerp(EndOffset, InitialOffset, EnePercentage);

        Plr.DOLocalMoveY(PlrPos, 0.3f).SetEase(Ease.OutQuad);
        Ene.DOLocalMoveY(EnePos, 0.3f).SetEase(Ease.OutQuad);

        /*
                float DiscardPercentage = InitialOffset * (1 + (Damage.discardDeck.Count / 52f) + (1 - Mathf.Clamp01(Damage.discardDeck.Count)));
        float DrawPercentage = InitialOffset * (1 + (Damage.drawDeck.Count / 52f) + (1 - Mathf.Clamp01(Damage.drawDeck.Count)));
        if (Damage.discardDeck.Count >= 52)
        {
            DrawPercentage = InitialOffset;
        }

        Draw.DOLocalMoveY(DiscardPercentage, 0.3f).SetEase(Ease.OutQuad);
        Discard.DOLocalMoveY(DrawPercentage, 0.3f).SetEase(Ease.OutQuad);
        */
    }
}
