using UnityEngine;
using DG.Tweening;

public class EnemyVisualizer : MonoBehaviour
{
    RectTransform transform;
    [HideInInspector] public bool isVisible = false;

    private void Start()
    {
        transform = GetComponent<RectTransform>();
    }
    public void ToggleView(bool toggle)
    {
        isVisible = toggle;
        if (toggle)
        {
            transform.DOLocalMoveY(540, 0.5f).SetEase(Ease.InOutQuad);
        }
        else
        {
            transform.DOLocalMoveY(730, 0.5f).SetEase(Ease.InOutQuad);
        }
    }
}
