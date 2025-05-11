using UnityEngine;
using DG.Tweening;

public class EnemyVisualizer : MonoBehaviour
{
    RectTransform transform;
    public bool isVisible = false;

    bool set = false;

    private void Start()
    {
        transform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (isVisible && !set)
        {
            set = true;
            ToggleView(true);
        }
        else if (!isVisible && set)
        {
            set = false;
            ToggleView(false);
        }
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
