using UnityEngine;
using DG.Tweening;
using System.Collections;

public class textshow : MonoBehaviour
{
    RectTransform root;
    private void Awake()
    {
        root = GetComponent<RectTransform>();
        root.localScale = Vector3.zero;
    }
    private void Start()
    {
        SetAppearance(true);
    }
    public void SetAppearance(bool toggle)
    {
        if (toggle)
        {
            root.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutBack);
        } else
        {
            root.DOScale(Vector3.zero, 0.75f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        }
    }
}
