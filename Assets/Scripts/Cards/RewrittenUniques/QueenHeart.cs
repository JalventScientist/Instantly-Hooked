using UnityEngine;

public class QueenHeart : Card
{
    public int AmountToHeal = 5;
    EvaluateDamage EvalDamage;

    private void Start()
    {
        WillBeAssigned = false;
        EvalDamage = FindFirstObjectByType<EvaluateDamage>();
    }

    public override void ApplyCard()
    {
        if (IsPlayerCard)
        {
            EvalDamage.plrHealth += AmountToHeal;
        } else
        {
            EvalDamage.enemyHealth += AmountToHeal;
        }
        base.ApplyCard();
    }
}
