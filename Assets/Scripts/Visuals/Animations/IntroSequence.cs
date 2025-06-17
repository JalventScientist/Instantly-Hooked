using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IntroSequence : MonoBehaviour
{

    [SerializeField] DialogueSystem dialog;

    [SerializeField] List<string> Dialogue = new List<string>();

    ReadDeck GameStarter;
    bool CheckingDialogue = false;

    [SerializeField] Transform Chair;

    TutorialLogger tutlog;
    private void Start()
    {
        tutlog = FindFirstObjectByType<TutorialLogger>();
        GameStarter = GetComponent<ReadDeck>();
        if(tutlog.IncludeTutorial == false)
        {
            Dialogue.Clear();
            Dialogue.Add("Welcome back. Let's get started.");
        }

        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(2.75f);
        Chair.DOLocalMove(new Vector3(1.41999996f, -4.98999977f, -6), 0.75f).SetEase(Ease.InOutSine); 
        Chair.DOLocalRotate(new Vector3(270, 123.112358f, 0), .75f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(0.75f);
        dialog.DialogueSequence(Dialogue);
        yield return new WaitUntil(() => dialog.DialogueFinished);
        FindFirstObjectByType<GameEnd>().TimerActive = true;
        GameStarter.Init();
        enabled = false;
    }
}
