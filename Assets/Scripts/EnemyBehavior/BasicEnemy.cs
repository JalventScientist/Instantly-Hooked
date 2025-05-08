using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class BasicEnemy : MonoBehaviour
{
    ReadDeck readDeck;

    List<GameObject> localDeck = new List<GameObject>(); //raw
    [SerializeField] List<GameObject> SpecialDeck = new List<GameObject>(); //filtered
    [SerializeField] List<GameObject> NormalDeck = new List<GameObject>();//filtered
    [SerializeField] List<GameObject> EarlyDeck = new List<GameObject>(); //filtered, contains cards that can be used pre-move

    bool SpecialCardInDeck = false;
    
    EvaluateDamage evaluateDamage;

    List<string> SpecialsPreMove = new List<string>() {"AH", "QH", "KC", "QC", "JC", "JS", "AD", "KD", "QD", "JD"}; //Cards that can be used pre-move

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
                if (SpecialsPreMove.Contains(card.name))
                {
                    EarlyDeck.Add(card);
                }
                else
                {
                    SpecialDeck.Add(card);
                }
            } else
            {
                NormalDeck.Add(card);
            }
        }
        if (SpecialDeck.Count > 0)
        {
            SpecialCardInDeck = true;
        }
        StartCoroutine(ChooseEarlyCardDelayed());
    }

    IEnumerator ChooseEarlyCardDelayed()
    {
        int Chance = EarlyDeck.Count > 0 ? Random.Range(0, 2) : 0; //50% chance to use a card if there are cards in the early deck
        if(Chance > 0)
        {
            yield return new WaitForSeconds(Random.Range(Random.Range(0.5f, 1.5f), Random.Range(1.5f, 2.5f)));
            ChooseEarlyCard();
        } else
        {
            readDeck.SetCardActivity(true);
            yield return null;
        }


    }

    public void ChooseEarlyCard()
    {
        bool ProperCardPicked = false;
        GameObject EarlyCard = null;
        while (!ProperCardPicked)
        {
            int cardIndex = Random.Range(0, EarlyDeck.Count);
            if ((EarlyDeck[cardIndex].name == "AH" || EarlyDeck[cardIndex].name == "QH") && evaluateDamage.enemyHealth >= 50)
            { //like why would you pull a health card if you're not damaged bruh
               if(EarlyDeck.Count == 1)
                {
                    readDeck.SetCardActivity(true);
                    return;
                } else
                {
                    continue;
                }
            } else if (EarlyDeck[cardIndex].name == "QC" && readDeck.MaxPlayerDeck <= 3) 
            { //Keep the game fair.
                if (EarlyDeck.Count == 1)
                {
                    readDeck.SetCardActivity(true);
                    return;
                }
                else
                {
                    continue;
                }
            }
            else
            {
                EarlyCard = EarlyDeck[cardIndex];
                ProperCardPicked = true;
                break;
            }
        }
        GameObject card = Instantiate(EarlyCard, new Vector3(0, 0, 0), Quaternion.identity);
        card.GetComponent<Card>().ApplyCard();
        EarlyDeck.Remove(EarlyCard);
        readDeck.SetCardActivity(true);
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
