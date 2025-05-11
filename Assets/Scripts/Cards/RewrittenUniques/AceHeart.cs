using UnityEngine;

public class AceHeart : Card
{
    EvaluateDamage evalDamage;

    public void Start()
    {
        WillBeAssigned = false;
        evalDamage = FindFirstObjectByType<EvaluateDamage>();
    }

    public override void ApplyCard()
    {
        if (IsPlayerCard && evalDamage.TargetedPlayer)
        {
            evalDamage.plrHealth += evalDamage.LastDealtDamage;
        }
        else if (!IsPlayerCard && !evalDamage.TargetedPlayer)
        {
            evalDamage.enemyHealth += evalDamage.LastDealtDamage;
        }
        base.ApplyCard();
    }
}
