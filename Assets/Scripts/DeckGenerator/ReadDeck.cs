using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadDeck : MonoBehaviour
{
    Decks cardDeck;

    public List<GameObject> PlayerDeck = new List<GameObject>();
    public List<GameObject> EnemyDeck = new List<GameObject>();

    [SerializeField] RectTransform PlayerDeckRender;
    [SerializeField] RectTransform EnemyDeckRender;

    public int MaxPlayerDeck = 5;
    public int MaxEnemyDeck = 5;

    private void Start()
    {
        cardDeck = GetComponent<Decks>();
        if (cardDeck == null)
        {
            Debug.LogError("Decks component not found in the scene.");
            return;
        } else
        {
            StartCoroutine(GetBasePull());
        }
    }

    IEnumerator GetBasePull()
    {
        yield return new WaitForSeconds(0.5f);
        for(int i = 0; i < 5; i++)
        {
            PullDeck(true);
            yield return new WaitForSeconds(0.05f);
            PullDeck(false);
            yield return new WaitForSeconds(0.05f);
        }
        FindFirstObjectByType<BasicEnemy>().GetDeck();

    }

    public IEnumerator PullOneSide(int count, bool ForPlayer)
    {
        for (int i = 0; i < count; i++)
        {
            PullDeck(ForPlayer);
            yield return new WaitForSeconds(0.05f);
        }
        if (!ForPlayer)
        {
            FindFirstObjectByType<BasicEnemy>().GetDeck();
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
            cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/Card");
        } else
        {
            cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/Card");
            //cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/UniqueCards" + SelectedCard);
        }
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

    IEnumerator UpdateDecksAnimated()
    {
        ThrowCard[] ThrownCards = FindObjectsByType<ThrowCard>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach(ThrowCard card in ThrownCards)
        {
            yield return new WaitForSeconds(Random.Range(0.01f, 0.05f));
            card.DestroyCard();
        }
        foreach (GameObject card in PlayerDeck)
        {
            if (card.GetComponent<Card>().UsedCard)
            {
                PlayerDeck.Remove(card);
                Destroy(card);
            }
        }

        yield return null;
    }
}
