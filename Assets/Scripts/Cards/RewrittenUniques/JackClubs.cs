using UnityEngine;

public class JackClubs : Card
{
    EnemyVisualizer enemyVisualizer;

    void Start()
    {
        WillBeAssigned = false;
        enemyVisualizer = FindFirstObjectByType<EnemyVisualizer>();
    }

    public override void ApplyCard()
    {
        base.ApplyCard();
        enemyVisualizer.ToggleView();
    }
}
