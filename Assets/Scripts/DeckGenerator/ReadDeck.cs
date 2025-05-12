using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReadDeck : MonoBehaviour
{
    Decks cardDeck;
    EnemyVisualizer EnemyHand;

    public List<GameObject> PlayerDeck = new List<GameObject>();
    public List<GameObject> EnemyDeck = new List<GameObject>();

    [SerializeField] RectTransform PlayerDeckRender;
    [SerializeField] RectTransform EnemyDeckRender;

    public int MaxPlayerDeck = 5;
    public int MaxEnemyDeck = 5;

    bool Reshuffling = false;

    //DELAYS
    WaitForSeconds unloadCards = new WaitForSeconds(0.05f);
    WaitForSeconds DefaultDelay = new WaitForSeconds(0.5f);
    WaitForFixedUpdate waitFixed = new WaitForFixedUpdate();

    private void Start()
    {
        EnemyHand = FindFirstObjectByType<EnemyVisualizer>();
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
        yield return DefaultDelay;
        for(int i = 0; i < MaxPlayerDeck; i++)
        {
            PullDeck(true);
            yield return unloadCards;
        }
        for (int i = 0; i < MaxEnemyDeck; i++)
        {
            PullDeck(false);
            yield return unloadCards;
        }
        Reshuffling = false;
        CheckDecks();
        FindFirstObjectByType<BasicEnemy>().GetDeck();
        SetCardActivity();
        
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

    public IEnumerator PullOneSide(int count, bool ForPlayer, bool IsMidGame = false)
    {
        for (int i = 0; i < count; i++)
        {
            PullDeck(ForPlayer);
            yield return unloadCards;
        }
        CheckDecks();
        if (!ForPlayer)
        {
            FindFirstObjectByType<BasicEnemy>().GetDeck(!IsMidGame);
        }
        else
        {
            if(IsMidGame)
            {
                SetCardActivity(true);
            }
        }

    }

    public IEnumerator DiscardOneSide(int count, bool ForPlayer, bool IsMidGame = false)
    {
        List<GameObject> TargetDeck = ForPlayer ? PlayerDeck : EnemyDeck;

        for (int i = 0; i < count && i < TargetDeck.Count; i++)
        {
            if (TargetDeck[0] != null)
            {
                TargetDeck[0].GetComponent<Card>().ForceRemoveCards();
                if (ForPlayer)
                {
                    PlayerDeck.RemoveAt(0);
                }
                else
                {
                    EnemyDeck.RemoveAt(0);
                }
                yield return waitFixed;
            }
        }
        if (!ForPlayer)
        {
            FindFirstObjectByType<BasicEnemy>().GetDeck(false);
        }
        yield return null;
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
        if (int.TryParse(strings[0], out int number)) //Special cards don't start with a number
        {
            cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/Card");
        } else
        {
            try
            {
                cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/UniqueCards/" + SelectedCard);
                if (cardPrefab.IsUnityNull())
                {
                    cardPrefab = Resources.Load<GameObject>("Prefabs/Cards/Card");
                }
            }
            catch
            {
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
        Reshuffling = true;
        SetCardActivity(false);
        yield return unloadCards;
        ThrowCard[] ThrownCards = FindObjectsByType<ThrowCard>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        List<GameObject> tempPlayerDeck = PlayerDeck;
        List<GameObject> tempEnemyDeck = EnemyDeck;
        foreach (ThrowCard card in ThrownCards)
        {
            yield return new WaitForSeconds(Random.Range(0.01f, 0.05f));
            card.DestroyCard();
        }
        print("clearing full deck");
        foreach (GameObject card in tempPlayerDeck)
        {
            if (!card.IsUnityNull())
            {
                PlayerDeck[PlayerDeck.IndexOf(card)].GetComponent<Card>().ForceRemoveCards();
            }
            else
            {
                print("GameObject is destroyed or smth idkf");
            }
            yield return waitFixed;
        }
        Card tempCard;
        foreach (GameObject card in tempEnemyDeck)
        {
            if(card.TryGetComponent<Card>(out tempCard))
            {
                card.GetComponent<Card>().ForceRemoveCards();
            } else
            {
                print("Can't find card script :((((((((((((((((");
            }
                yield return waitFixed;
        }
        print("we've reached this point");
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

    void DiscardToDraw()
    {
        List<string> tempDeck = new List<string>();
        foreach (string card in cardDeck.discardDeck)
        {
            tempDeck.Add(card);
        }
        for (int i = 0; i < tempDeck.Count; i++)
        {
        }
        foreach (string card in tempDeck)
        {
            if(PlayerDeck.Exists(c => c.name == card) || EnemyDeck.Exists(c => c.name == card)) //check if card isn't in use by enemy or player to prevent accidental duplication
            {
                continue;
            } else
            {
                cardDeck.discardDeck.Remove(card);
            }
        }
    }

    void CheckDecks(bool DontCorrect = false)// Prevents that a deck has only Special cards (unless the player has Diamond Ace, King or jack since those change the decks)
    {
        int TotalShortage = PlayerDeck.Count + EnemyDeck.Count - (MaxEnemyDeck + MaxPlayerDeck); //calculate how many cards need to be pulled

        if(TotalShortage > cardDeck.drawDeck.Count)
        {
            print("Too few cards to draw. Moving discard pile to draw.");
            DiscardToDraw();
            return;
        }

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
                print("Reshuffling decks");
                StartCoroutine(RenewDecks());
            }
        }
    }


    IEnumerator UpdateDecksAnimated(bool GetNewCards = true)
    {
        yield return DefaultDelay;
        if (EnemyHand.isVisible)
        {
            EnemyHand.ToggleView(false);
        }
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
        yield return new WaitForSeconds(0.5f);
        CheckDecks();
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
