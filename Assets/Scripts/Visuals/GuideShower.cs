using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GuideShower : MonoBehaviour
{
    bool ShowingGuide = false;
    RectTransform guideRectTransform;

    [SerializeField] Button _Button;

    public void ShowGuideOption(bool toggle = true)
    {
        _Button.enabled = toggle;
        guideRectTransform.localPosition = toggle ? new Vector3(447, 0, 0) : new Vector3(500,0,0);
    }

    private void Start()
    {
        _Button.enabled = false;
        guideRectTransform = GetComponent<RectTransform>();
        guideRectTransform.localPosition = new Vector3(500, 0, 0);
    }

    public void ToggleGuide()
    {
        ShowingGuide = !ShowingGuide;
        if (ShowingGuide)
        {
            guideRectTransform.DOLocalMoveX(190, 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            guideRectTransform.DOLocalMoveX(447, 0.5f).SetEase(Ease.InBack);
        }
    }
}
