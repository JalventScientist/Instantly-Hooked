using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EnemyVisualizer : MonoBehaviour
{
    ReadDeck Deck;
    public bool isVisible = false;
    HorizontalLayoutGroup layoutGroup;

    [SerializeField] Animator animator;

    bool set = false;

    private void Start()
    {
        Deck = FindFirstObjectByType<ReadDeck>();
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
    }

    private void Update()
    {
        if (isVisible)
        {
            ToggleView();
        }
        isVisible = false;
    }

    IEnumerator displayCards()
    {
        bool clickclack = true;
        animator.SetBool("ViewCards", true);
        for (int i = 0; i< 2; i++)
        {
            WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
            DOTween.To(() => layoutGroup.spacing, x => layoutGroup.spacing = x, clickclack ? 0 : -69, .5f).SetEase(Ease.InOutSine);

            foreach (GameObject card in Deck.PlayerDeck)
            {
                Card script = card.GetComponent<Card>();
                script.TurnCard(clickclack);
                yield return fixedUpdate;
            }
            animator.SetBool("ViewCards", false);
            yield return new WaitForSeconds(0.5f);
            foreach (GameObject card in Deck.EnemyDeck)
            {
                Card script = card.GetComponent<Card>();
                script.TurnCard(clickclack);
                yield return fixedUpdate;
            }
            clickclack = !clickclack;
            yield return new WaitForSeconds(2);
        }


    }

    public void ToggleView()
    {
        StartCoroutine(displayCards());
    }
}
