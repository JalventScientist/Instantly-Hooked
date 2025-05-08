using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    bool Reshuffling = false;

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
        for(int i = 0; i < MaxPlayerDeck; i++)
        {
            PullDeck(true);
            yield return new WaitForSeconds(0.05f);
        }
        for (int i = 0; i < MaxEnemyDeck; i++)
        {
            PullDeck(false);
            yield return new WaitForSeconds(0.05f);
        }
        FindFirstObjectByType<BasicEnemy>().GetDeck();
        CheckDecks();
        SetCardActivity();
        Reshuffling = false;
    }

    public void SetCardActivity(bool toggle = true) //Toggles if cards can be used or not to prevent premature clicks
    {
        foreach (GameObject card in PlayerDeck)
        {
            card.GetComponent<Card>().TrulyActive = toggle;
        }
        foreach (GameObject card in EnemyDeck)
        {
            card.GetComponent<Card>().TrulyActive = toggle;
        }
    }

    public IEnumerator PullOneSide(int count, bool ForPlayer)
    {
        print("Called");
        for (int i = 0; i < count; i++)
        {
            PullDeck(ForPlayer);
            yield return new WaitForSeconds(0.05f);
        }
        if (!ForPlayer)
        {
            FindFirstObjectByType<BasicEnemy>().GetDeck();
        }
        CheckDecks();
    }

    public void PullDeck(bool playerCard)
    {
        string SelectedCard = "";
        SelectedCard = cardDeck.drawDeck[cardDeck.drawDeck.Count - 1];

        cardDeck.drawDeck.Remove(SelectedCard);
        List<string> strings = new List<string>();
        foreach(char c in SelectedCard)
        {
            strings.Add(c.ToString());
        }
        GameObject cardPrefab;
        print(strings[0] + " of " + SelectedCard);
        if (int.TryParse(strings[0], out int number)) //Special cards don't start with a number
        {
            cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/Card");
        } else
        {
            try
            {
                print("Trying to receive");
                cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/UniqueCards/" + SelectedCard);
                if (cardPrefab.IsUnityNull())
                {
                    Debug.LogError("Card prefab not found: " + SelectedCard);
                    cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/Card");
                }
            }
            catch
            {
                Debug.LogError("Card is not found, or card format is invalid: " + SelectedCard);
                cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/Card");
            }
            
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

    public void Regen()
    {
        StartCoroutine(UpdateDecksAnimated());
    }

    public IEnumerator RenewDecks()
    {
        SetCardActivity(false);
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject card in PlayerDeck)
        {
            StartCoroutine(card.GetComponent<Card>().ForceRemoveCards()); ;
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < EnemyDeck.Count; i++)
        {
            if (EnemyDeck[i].TryGetComponent<Card>(out Card cardScript))
            {
                StartCoroutine(cardScript.ForceRemoveCards());
            }
            yield return new WaitForSeconds(0.01f);
        }
        PlayerDeck.Clear();
        EnemyDeck.Clear();
        List<string> tempDeck = new List<string>();
        foreach (string card in cardDeck.drawDeck)
        {
            tempDeck.Add(card);
        }
        foreach (string card in cardDeck.discardDeck)
        {
            tempDeck.Add(card);
        }
        cardDeck.drawDeck.Clear();
        cardDeck.discardDeck.Clear();
        Util.ShuffleFromDeckIntoDeck(tempDeck, cardDeck.drawDeck);
        StartCoroutine(GetBasePull());
    }

    void CheckDecks()// Prevents that a deck has only Special cards (unless the player has Diamond Ace, King or jack since those change the decks)
    {
        bool PlayerHasOnlySpecial = true;
        bool EnemyHasOnlySpecial = true;

        foreach (GameObject card in PlayerDeck)
        {
            switch (card.name)
            {
                case "AD":
                    PlayerHasOnlySpecial = false;
                    break;
                case "AK":
                    PlayerHasOnlySpecial = false;
                    break;
                case "AJ":
                    PlayerHasOnlySpecial = false;
                    break;
            }
            if(card.GetComponent<Card>().uniqueCard == Uniquecard.None && PlayerHasOnlySpecial) //Incase the player has a normal card
            {
                PlayerHasOnlySpecial = false;
            }
        }

        foreach (GameObject card in EnemyDeck)
        {
            switch (card.name)
            {
                case "AD":
                    EnemyHasOnlySpecial = false;
                    break;
                case "AK":
                    EnemyHasOnlySpecial = false;
                    break;
                case "AJ":
                    EnemyHasOnlySpecial = false;
                    break;
            }
            if (card.GetComponent<Card>().uniqueCard == Uniquecard.None && EnemyHasOnlySpecial) //Incase the player has a normal card
            {
                EnemyHasOnlySpecial = false;
            }
        }

        if(PlayerHasOnlySpecial || EnemyHasOnlySpecial)
        {
            if (!Reshuffling)
            {
                Reshuffling = true;
                StartCoroutine(RenewDecks());
            }
        } else
        {
            SetCardActivity(true);
        }
    }



    IEnumerator UpdateDecksAnimated(bool GetNewCards = true)
    {
        yield return new WaitForSeconds(0.5f);
        SetCardActivity(false);
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
                Destroy(card.gameObject);
            }
        }
        foreach (GameObject card in EnemyDeck)
        {
            if (card.GetComponent<Card>().UsedCard)
            {
                Destroy(card.gameObject);
            }
        }
        if(GetNewCards)
        {
            yield return new WaitForSeconds(1f);
            UpdateCards();
        }
        yield return null;
    }

    void UpdateCards()
    {
        PlayerDeck.RemoveAll(card => card == null || card.Equals(null));
        EnemyDeck.RemoveAll(card => card == null || card.Equals(null));
        if (PlayerDeck.Count < MaxPlayerDeck)
        {
            int shortage = MaxPlayerDeck - PlayerDeck.Count;
            StartCoroutine(PullOneSide(shortage, true));
        }
        if (EnemyDeck.Count < MaxEnemyDeck)
        {
            int shortage = MaxEnemyDeck - EnemyDeck.Count;
            StartCoroutine(PullOneSide(shortage, false));
        }
    }
}
