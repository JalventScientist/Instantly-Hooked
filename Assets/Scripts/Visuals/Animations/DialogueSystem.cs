using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{

    [SerializeField] TMP_Text DialogueText;
    List<string> DialogueList = new List<string>();

    public bool DialogueFinished = false;
    bool HitSkip = false;
    bool WaitingForNext = false;

    bool Typing = false;
    bool Debounce = false;

    int currentListIndex = 0;
    bool WaitingForNextList = true;

    public bool AlternativeClick = false; // If true, will use alternative click method instead of left mouse button
    [SerializeField] Button AlternativeClickButton; // Button to use for alternative click method

    public void SetAltClick(bool toggle)
    {
        AlternativeClick = toggle;
    }

    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!WaitingForNextList)
        {
            if((Input.GetMouseButtonDown(0) || AlternativeClick) && !Debounce)
            {
                if (!AlternativeClick)
                    Debounce = true;
                AlternativeClick = false;

                if (WaitingForNext)
                {
                    if (currentListIndex < DialogueList.Count - 1)
                    {
                        currentListIndex++;
                        StartCoroutine(TypeWriteAnim(DialogueList[currentListIndex], 0.05f));
                    }
                    else
                    {
                        DialogueFinished = true;
                        WaitingForNextList = true;
                        AlternativeClickButton.gameObject.SetActive(false);
                        mainCamera.DOFieldOfView(60, 0.5f).SetEase(Ease.InOutSine);
                        DialogueText.DOColor(new Color(1, 1, 1, 0), 0.5f).SetEase(Ease.InOutSine).OnComplete(() => 
                        {
                            DialogueText.text = "";
                        });
                    }
                }
                else
                {
                    HitSkip = true;
                }
            }
            if (Debounce && Input.GetMouseButtonUp(0))
            {
                Debounce = false;
            }
        }

    }

    public void SingleDialog(string _text, float delay = 0.05f)
    {
        StartCoroutine(TypeWriteSingle(_text, delay));
    }

    public void DialogueSequence(List<string> _list)
    {
        mainCamera.DOFieldOfView(52, 0.5f).SetEase(Ease.InOutSine);
        AlternativeClickButton.gameObject.SetActive(true);
        DialogueList = _list;
        DialogueFinished = false;
        if (DialogueList.Count > 0)
        {
            WaitingForNextList = false;
            currentListIndex = 0;
            StartCoroutine(TypeWriteAnim(DialogueList[currentListIndex], 0.05f));
        }
    }

    IEnumerator TypeWriteAnim(string text, float delay)
    {
        DialogueText.color = new Color(1, 1, 1, 1);
        DialogueText.text = "";
        Debounce = true;
        WaitingForNext = false;
        WaitForSeconds wait = new WaitForSeconds(delay);
        for (int i = 0; i < text.Length; i++)
        {
            if (!HitSkip)
            {
                DialogueText.text += text[i];
                yield return new WaitForSeconds(delay);
            }
            else
            {
                break;
            }
        }
        HitSkip = false;
        WaitingForNext = true;
        DialogueText.text = text + "<size=15><color=grey>(click to continue)";
    }

    IEnumerator TypeWriteSingle(string text, float delay)
    {
        DialogueText.color = new Color(1, 1, 1, 1);
        DialogueText.text = "";
        for (int i = 0; i < text.Length; i++)
        {
                DialogueText.text += text[i];
                yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds((text.Length / 20) + 0.5f);
        DialogueText.DOColor(new Color(1, 1, 1, 0), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            DialogueText.text = "";
        });
    }
}
