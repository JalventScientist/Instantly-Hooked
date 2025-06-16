using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameEnd : MonoBehaviour
{
    RectTransform rectTransform;

    float TimeElapsed = 0f; //count total match time


    [SerializeField] TMP_Text WinLose; //Indicate if the player won or lost
    [SerializeField] TMP_Text TimeText; //Display the total time elapsed in the match
    [SerializeField] TMP_Text cardText; //Display how many cards the player has used the entire match

    public bool TimerActive = false;


    private void Update()
    {
        if (TimerActive)
        {
            TimeElapsed += Time.deltaTime;
        }
    }

    public void SubmitResults()
    {
        int minutes = Mathf.FloorToInt(TimeElapsed / 60f);
        int seconds = Mathf.FloorToInt(TimeElapsed % 60f);
        string TotalTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        TimeText.text = "Time: " + TotalTime;
    }
}
