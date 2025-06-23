using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameEnd : MonoBehaviour
{
   [SerializeField] RectTransform rectTransform;

    float TimeElapsed = 0f; //count total match time


    public TMP_Text WinLose; //Indicate if the player won or lost
    [SerializeField] TMP_Text TimeText; //Display the total time elapsed in the match
    [SerializeField] TMP_Text cardText; //Display how many cards the player has used the entire match

    public bool TimerActive = false;
    public int CardsUsed = 0;

    [SerializeField] Image backFade;
    [SerializeField] Image frontFade;

    bool InputDebounce = true; //Prevents premature input

    private void Start()
    {
        backFade.gameObject.SetActive(false);
    }

    public void ToScene(bool MenuOrRestart)
    {
        if (!InputDebounce)
        {
            frontFade.gameObject.SetActive(true);
            InputDebounce = true;
            frontFade.DOFade(1f, 1f).OnComplete(() =>
            {
                if (MenuOrRestart)
                {
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            });
        }

    }

    public IEnumerator FadeIn()
    {
        backFade.gameObject.SetActive(true);

        backFade.DOFade(0.5f, 1f);
        yield return new WaitForSeconds(0.5f);
        rectTransform.DOLocalMove(Vector3.zero, 1f);
        yield return new WaitForSeconds(1f);
        InputDebounce = false;
    }


    private void Update()
    {
        if (TimerActive)
        {
            TimeElapsed += Time.deltaTime;
        }
    }

    public void SubmitResults()
    {
        TimerActive = false;
        int minutes = Mathf.FloorToInt(TimeElapsed / 60f);
        int seconds = Mathf.FloorToInt(TimeElapsed % 60f);
        string TotalTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        TimeText.text = "Time: " + TotalTime;
        cardText.text = "Cards Played: " + CardsUsed.ToString();
    }
}
