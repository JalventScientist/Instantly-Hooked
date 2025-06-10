using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{

    [SerializeField] DialogueSystem dialog;

    List<string> Dialogue = new List<string>(3);

    ReadDeck GameStarter;
    bool CheckingDialogue = false;

    private void Awake()
    {
        Dialogue[1] = "A newcomer, I presume.";
        Dialogue[2] = "Let's get you settled with a match.";
        Dialogue[3] = "We'll start simple; no face cards, just pick the highest you have.";
    }

    private void Update()
    {
        if (dialog.DialogueFinished && CheckingDialogue)
        {
            GameStarter.Init();
            enabled = false;
        }
    }

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(1f);
        dialog.DialogueSequence(Dialogue);
    }
}
