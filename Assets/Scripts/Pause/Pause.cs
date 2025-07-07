using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Pause : MonoBehaviour
{
   [SerializeField] RectTransform rectTransform;
    [SerializeField] GameEnd Timer;

    [SerializeField] Image backFade;
    [SerializeField] Image frontFade;

    public bool InputDebounce = true; //Prevents premature input
    bool toggled = false;

    private void Start()
    {
        backFade.gameObject.SetActive(false);
        frontFade.gameObject.SetActive(false);
    }

    public void ToScene()
    {
        if (!InputDebounce)
        {
            frontFade.gameObject.SetActive(true);
            InputDebounce = true;
            frontFade.DOFade(1f, 1f).OnComplete(() =>
            {
               SceneManager.LoadScene("MainMenu");
            });
        }

    }

    public IEnumerator ToggleFade(bool toggle)
    {
        if (toggle)
        {
            backFade.gameObject.SetActive(true);
            backFade.DOFade(0.5f, 1f);
            rectTransform.DOLocalMove(Vector3.zero, 1f);
            yield return new WaitForSeconds(1f);
            InputDebounce = false;
            toggled = true;
        } else
        {
            toggled = false;
            rectTransform.DOLocalMoveY(960f, 1f);
            backFade.DOFade(0.5f, 0.5f);
            yield return new WaitForSeconds(0.5f);
            backFade.gameObject.SetActive(false);
            InputDebounce = false;
        }
    }

    public void SetState(bool toggle)
    {
        if (!InputDebounce)
        {
            InputDebounce = true; // Prevents multiple inputs while the fade is in progress
            if (toggle)
            {
                Timer.TimerActive = false;
                StartCoroutine(ToggleFade(true));
            }
            else
            {
                Timer.TimerActive = true;
                StartCoroutine(ToggleFade(false));
            }
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            print("AH HIT");
            if (toggled == false)
            {
                SetState(true);
            }
            else
            {
                SetState(false);
            }
        }
    }

}
