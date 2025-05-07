using UnityEngine;

public class QueenOfHearts : Card
{
    private EvaluateDamage health;
    public int restoreAmount;

    private void Start()
    {
        health = FindAnyObjectByType<EvaluateDamage>();
    }
    public override void ApplyCard()
    {
        base.ApplyCard();
        /*if (IsPlayerCard)
            health.plrHealth += restoreAmount;
        else
            health.enemyHealth += restoreAmount;*/
    }
}
