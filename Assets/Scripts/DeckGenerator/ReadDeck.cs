using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReadDeck : MonoBehaviour
{

    [Header("Generation")]
    [SerializeField] private List<string> allCards = new List<string>();
    public List<string> drawDeck = new List<string>();
    public List<string> discardDeck = new List<string>();
    [HideInInspector] public List<string>[] drawAndDiscardDeck = new List<string>[2];


    DeckVisualizer DeckAnimator;

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

    [SerializeField] Transform cam;
    [SerializeField] GameObject shuffleIndicator;

    //To gradually add face cards:
    [SerializeField] List<bool> enabledFaces = new List<bool>(4); //1 = Spades, 2 = Hearts, 3 = Diamonds, 4 = Clubs. If a face card is enabled, it will be added to the deck.
    DialogueSystem dialogSystem;

    TutorialLogger tutlog;

    private void Awake()
    {
        tutlog = FindFirstObjectByType<TutorialLogger>();
        dialogSystem = GetComponent<DialogueSystem>();
        DeckAnimator = FindFirstObjectByType<DeckVisualizer>();
        drawAndDiscardDeck[0] = drawDeck;
        drawAndDiscardDeck[1] = discardDeck;
        Util.ShuffleFromDeckIntoDeck(allCards, drawDeck);
        DeckAnimator.UpdateDecks();
    }
    public void Init()
    {
        if(tutlog.IncludeTutorial == false)
        {
            for(int i = 0; i < enabledFaces.Count; i++)
            {
                enabledFaces[i] = true; //Enable all face cards
            }
        }
        StartCoroutine(GetBasePull());
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

    IEnumerator shakeCam()
    {
        shuffleIndicator.SetActive(true);
        for(int i = 0; i < 10; i++)
        {
            cam.localPosition = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f,0.1f));
            yield return waitFixed;
        }
        cam.localPosition = Vector3.zero;
        yield return new WaitForSeconds(1);

        shuffleIndicator.SetActive(false);
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
        print("Activated all cards");
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

    public IEnumerator DiscardOneSide(int count, bool ForPlayer, bool forceLock = false, bool RenewAfterwards = false)
    {

        List<GameObject> TargetDeck = ForPlayer ? PlayerDeck : EnemyDeck;

        if (forceLock)
        {
            SetCardActivity(false);
        }
        //Making sure the game doesn't discard more cards than there are available. Jack of Diamonds mainly relies on this.
        int chances = 0;
        bool ReachedHardEnd;
        List<GameObject> ActualDeck = new List<GameObject>(); //Deck with only active cards that can actually be pulled

        foreach(GameObject card in TargetDeck)
        {
            if (!card.GetComponent<Card>().UsedCard)
            {
                ActualDeck.Add(card);
            }
        }

        if(ActualDeck.Count < count)
        {
            Debug.LogError("No new Cards found");
            yield return null;
        }
        //Actually get rid of the cards
        for (int i = 0; i < count && i < ActualDeck.Count; i++)
        {
            if (TargetDeck.Contains(ActualDeck[i]))
            {
                int j = TargetDeck.IndexOf(ActualDeck[i]);
                TargetDeck[j].GetComponent<Card>().ForceRemoveCards();
                if (ForPlayer)
                {
                    PlayerDeck.RemoveAt(j);
                }
                else
                {
                    EnemyDeck.RemoveAt(j);
                }
                DeckAnimator.UpdateDecks();
                yield return waitFixed;
            }
        }
        if (!ForPlayer)
        {
            FindFirstObjectByType<BasicEnemy>().GetDeck(false);
        }
        if (RenewAfterwards)
        {
            StartCoroutine(PullOneSide(count, ForPlayer, true));
        }
        yield return null;
    }

    public void PullDeck(bool playerCard)
    {
        string SelectedCard = "";
        List<string> tempDeck = new List<string>(drawDeck); // Used to filter out unused face cards

        foreach(string _card in drawDeck)
        {
            List<string> str = new List<string>();
            foreach (char c in _card)
            {
                str.Add(c.ToString());
            }
            if (int.TryParse(str[0], out int num))
            {
                continue; //If the card is a number, skip it because number cards are guaranteed to be played
            } else
            {
                int faceCardIndex = 0;
                switch (str[1])
                {
                    //Check the suit of the card
                    case "S":
                        faceCardIndex = 0; //Spades
                        break;
                    case "H":
                        faceCardIndex = 1; //Hearts
                        break;
                    case "D":
                        faceCardIndex = 2; //Diamonds
                        break;
                    case "C":
                        faceCardIndex = 3; //Clubs
                        break;
                }
                if(enabledFaces[faceCardIndex] == false) //If the face card is enabled, add it to the tempDeck
                {
                    tempDeck.Remove(_card);
                }
            }
        }


        // Check if drawDeck is not empty before accessing the last element
        if (tempDeck.Count > 0)
        {
            SelectedCard = tempDeck[tempDeck.Count - 1];
        }
        else
        {
            StopAllCoroutines(); //Stop trying to pull
            Reshuffling = true;
            StartCoroutine(RenewDecks());
            return;
        }

        drawDeck.Remove(SelectedCard);
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
        discardDeck.Add(SelectedCard);
        DeckAnimator.UpdateDecks();
        cardScript.SetupCard();
    }

    public void EnableRandomFace()
    {
        List<bool> tempEnabledFaces = new List<bool>(enabledFaces);
        foreach (bool face in enabledFaces)
        {
            if(face == true)
            {
                tempEnabledFaces.Remove(face);
            }
        }
        if(tempEnabledFaces.Count > 0)
        {
            int randomIndex = Random.Range(0, tempEnabledFaces.Count);
            int TextIndex = enabledFaces.IndexOf(tempEnabledFaces[randomIndex]);
            enabledFaces[enabledFaces.IndexOf(tempEnabledFaces[randomIndex])] = true; //Enable a random face card
            switch (TextIndex)
            {
                case 0:
                    dialogSystem.SingleDialog("I've put the face cards of spades in.");
                    break;
                case 1:
                    dialogSystem.SingleDialog("I've put the face cards of hearts in.");
                    break;
                case 2:
                    dialogSystem.SingleDialog("I've put the face cards of diamonds in.");
                    break;
                case 3:
                    dialogSystem.SingleDialog("I've put the face cards of clubs in.");
                    break;
            }
        }
    }

    public void Regen(bool IncludeNewDeck = false)
    {
        if (IncludeNewDeck)
        {
            EnableRandomFace();
        }
        StartCoroutine(UpdateDecksAnimated());
    }

    public IEnumerator RenewDecks()
    {
        StartCoroutine(shakeCam());
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
        PlayerDeck.Clear();
        EnemyDeck.Clear();
        List<string> tempDeck = new List<string>();
        foreach (string card in drawDeck)
        {
            tempDeck.Add(card);
        }
        foreach (string card in discardDeck)
        {
            tempDeck.Add(card);
        }
        drawDeck.Clear();
        discardDeck.Clear();
        Util.ShuffleFromDeckIntoDeck(tempDeck, drawDeck);
        StartCoroutine(GetBasePull());
    }

    void DiscardToDraw()
    {
        List<string> tempDeck = new List<string>();
        foreach (string card in discardDeck)
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
                discardDeck.Remove(card);
            }
        }
    }

    void CheckDecks(bool DontCorrect = false)// Prevents that a deck has only Special cards (unless the player has Diamond Ace, King or jack since those change the decks)
    {
        int TotalShortage = PlayerDeck.Count + EnemyDeck.Count - (MaxEnemyDeck + MaxPlayerDeck); //calculate how many cards need to be pulled

        if(TotalShortage > drawDeck.Count)
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

    // FUNCTIONS FOR UPDATING CARDS POST-SHOWDOWN -------------------------------------------------------------------------
    IEnumerator UpdateDecksAnimated(bool GetNewCards = true)
    {
        yield return DefaultDelay;
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

    public IEnumerator ItsJoever()
    {
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
    }
}
