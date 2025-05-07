using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadDeck : MonoBehaviour
{
    Decks cardDeck;

    List<GameObject> PlayerDeck = new List<GameObject>();
    List<GameObject> EnemyDeck = new List<GameObject>();

    [SerializeField] RectTransform PlayerDeckRender;
    [SerializeField] RectTransform EnemyDeckRender;


    private void Start()
    {
        cardDeck = GetComponent<Decks>();
        if (cardDeck == null)
        {
            Debug.LogError("Decks component not found in the scene.");
            return;
        } else
        {
            StartCoroutine(GetPull());
        }
    }

    IEnumerator GetPull()
    {
        yield return new WaitForSeconds(0.5f);
        for(int i = 0; i < 5; i++)
        {
            PullDeck(true);
            yield return new WaitForSeconds(0.05f);
            PullDeck(false);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void PullDeck(bool playerCard)
    {
        string SelectedCard = "";
        SelectedCard = cardDeck.drawDeck[cardDeck.drawDeck.Count - 1];

        cardDeck.drawDeck.Remove(SelectedCard);
        string[] strings = SelectedCard.Split('_');
        GameObject cardPrefab;
        if (int.TryParse(strings[0], out int number)) //Special cards don't start with a number
        {
            print(":D");
            cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/Card");
        } else
        {
            print("D:");
            cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/Card");
            //cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/UniqueCards" + SelectedCard);
        }

        print(cardPrefab);
        GameObject card = Instantiate(cardPrefab, transform.position, Quaternion.identity);
        Card cardScript = card.GetComponent<Card>();
        card.name = SelectedCard;
        if (playerCard)
        {
            cardScript.IsPlayerCard = true;
            PlayerDeck.Add(card);
            card.transform.SetParent(PlayerDeckRender, false);
        }
        else
        {
            cardScript.IsPlayerCard = false;
            EnemyDeck.Add(card);
            card.transform.SetParent(EnemyDeckRender, false);
        }
        cardDeck.discardDeck.Add(SelectedCard);
        cardScript.SetupCard();
    }
}
