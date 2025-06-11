using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IntroSequence : MonoBehaviour
{

    [SerializeField] DialogueSystem dialog;

    List<string> Dialogue = new List<string>();

    ReadDeck GameStarter;
    bool CheckingDialogue = false;

    [SerializeField] Transform Chair;

    /*
     private void Awake()
    {
        Dialogue[1] = "A newcomer, I presume.";
        Dialogue[2] = "Let's get you settled with a match.";
        Dialogue[3] = "We'll start simple; no face cards, just pick the highest you have.";
    }
     */

    private void Start()
    {
        GameStarter = GetComponent<ReadDeck>();
        StartCoroutine(Sequence());
    }

    /*
     private void Update()
    {
        if (dialog.DialogueFinished && CheckingDialogue)
        {
            GameStarter.Init();
            enabled = false;
        }
    }
     */

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(3f);
        Chair.DOLocalMove(new Vector3(1.41999996f, -4.98999977f, -6), 0.75f).SetEase(Ease.InOutSine); 
        Chair.DOLocalRotate(new Vector3(270, 123.112358f, 0), .75f).SetEase(Ease.InOutSine);
        GameStarter.Init();
        //dialog.DialogueSequence(Dialogue);
    }
}
