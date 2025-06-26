using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuFunctions : MonoBehaviour
{
    [SerializeField] string TargetScene;
    [SerializeField] GameObject FadeUI;
    Image fadeimg;

    private void Awake()
    {
        fadeimg = FadeUI.GetComponent<Image>();
        FadeUI.SetActive(true);
        fadeimg.color = new Color(0, 0, 0, 1);
    }

    private void Start()
    {
        fadeimg.DOColor(new Color(0, 0, 0, 0), 1f).OnComplete(() =>
        {
            FadeUI.SetActive(false);
        });
    }

    public void LoadScene()
    {
        FadeUI.SetActive(true);
        Camera.main.transform.DOLocalMoveX(5.61f, 3f).SetEase(Ease.InOutQuad);
        Camera.main.transform.DOLocalRotate(new Vector3(-13.1f, 0, 0), 3f).SetEase(Ease.InOutQuad);
        fadeimg.DOColor(new Color(0, 0, 0, 1), 1f).OnComplete(() =>
        {
            SceneManager.LoadScene(TargetScene);
        });
    }
}
