using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BasicEnemy : MonoBehaviour
{
    ReadDeck readDeck;

    List<GameObject> localDeck = new List<GameObject>(); //raw
    [SerializeField] List<GameObject> SpecialDeck = new List<GameObject>(); //filtered
    [SerializeField] List<GameObject> NormalDeck = new List<GameObject>();//filtered

    bool SpecialCardInDeck = false;
    EvaluateDamage evaluateDamage;

    private void Start()
    {
        readDeck = FindFirstObjectByType<ReadDeck>();
        evaluateDamage = GetComponent<EvaluateDamage>();
    }

    public void GetDeck()
    {
        localDeck.Clear();
        SpecialDeck.Clear();
        NormalDeck.Clear();
        foreach (GameObject card in readDeck.EnemyDeck)
        {
            localDeck.Add(card);
        }
        SortCards();
    }

    void SortCards()
    {
        SpecialCardInDeck = false;
        foreach(GameObject card in localDeck)
        {
            if (card.GetComponent<Card>().uniqueCard != Uniquecard.None)
            {   
                SpecialDeck.Add(card);
            } else
            {
                NormalDeck.Add(card);
            }
        }
        if (SpecialDeck.Count > 0)
        {
            SpecialCardInDeck = true;
        }
    }

    public void SelectCard(bool SpecialAllowed = true)
    {
        print("Selecting Card");
        int SpecialOrNormal = SpecialCardInDeck && SpecialAllowed ? Random.Range(0,4) : 1;

        if (SpecialOrNormal > 0) //Use a normal card
        {
            int cardIndex = Random.Range(0, NormalDeck.Count);
            GameObject card = NormalDeck[cardIndex];
            NormalDeck.RemoveAt(cardIndex);
            card.GetComponent<Card>().ApplyCard();
            evaluateDamage.EvalDamage();

        } else //use a special card
        {
            int cardIndex = Random.Range(0, SpecialDeck.Count);
            GameObject card = SpecialDeck[cardIndex];
            SpecialDeck.RemoveAt(cardIndex);
            card.GetComponent<Card>().ApplyCard();
            SelectCard(false);
        }
        
    }

}
