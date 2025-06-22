using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using TMPro;
using System;
using DG.Tweening;

public enum CardType
{
    Heart,
    Spade,
    Diamond,
    Club
}
public class EvaluateDamage : MonoBehaviour
{
    List<Card> EnemyCard = new List<Card>();
    List<Card> PlayerCard = new List<Card>();

    List<Card> PlayerBuffCard = new List<Card>();
    List<Card> EnemyBuffCard = new List<Card>();

    BasicEnemy EnemyScript;
    ReadDeck ReadDeckScript;
    AnimateCam CamAnimator;
    GuideShower GuideShowerScript;
    HealthVisualiser HealthVisualiserScript;

    [SerializeField] TMP_Text PlayerHealthText;
    [SerializeField] TMP_Text EnemyHealthText;

    [SerializeField] private int extraDamageFromTypingAdvantage;

    public int plrHealth = 50;
    public int enemyHealth = 50;

    public bool PlayerHasExtraMove = false;
    public bool EnemyHasExtraMove = false;
    public int EnemyMoves = 1;

    public int EnemyCardsPlayed = 0;
    public int PlayerCardsPlayed = 0;

    public int LastDealtDamage = 0;
    public bool TargetedPlayer = false; //True = Player, False = Enemy

    bool TempDebounce = false; //Prevents useless waiting

    [Header("Components for eval")]
    [SerializeField] GameObject effectPreset;
    [SerializeField] RectTransform enemySide;
    [SerializeField] RectTransform playerSide;
    [SerializeField] RectTransform Center;

    [SerializeField] TMP_Text EnemyDam;
    [SerializeField] TMP_Text PlayerDam;
    [SerializeField] TMP_Text Total;


    WaitForSeconds def = new WaitForSeconds(1f);

    private char[] typeChart = { 'H', 'C', 'D', 'S' };

    List<textshow> BuffVisuals = new List<textshow>();
    DialogueSystem dialogSystem;

    TutorialLogger tutlog;

    [SerializeField] int TurnsUntilFace = 4;
    int usedTurns = 0;

    bool WaitExtra = false;

    int TotalCardsPLayed = 0;


    //FOR WIN/LOSE SEQUENCES

    bool WinCheck = false;
    bool WaitTilWin = false;
    bool Winside = false; //False = Enemy win, True = Player win

    GameEnd gameEndScript;

    private void Start()
    {
        tutlog = FindFirstObjectByType<TutorialLogger>();
        dialogSystem = GetComponent<DialogueSystem>();
        CamAnimator = FindFirstObjectByType<AnimateCam>();
        EnemyScript = GetComponent<BasicEnemy>();
        ReadDeckScript = GetComponent<ReadDeck>();
        gameEndScript = FindFirstObjectByType<GameEnd>();
        GuideShowerScript = FindFirstObjectByType<GuideShower>();
        HealthVisualiserScript = FindFirstObjectByType<HealthVisualiser>();
    }

    private void Update()
    {
        if (PlayerHealthText != null)
        {
            PlayerHealthText.text = "Player Health" + plrHealth.ToString();
        }
        if (EnemyHealthText != null)
        {
            EnemyHealthText.text = "Enemy Health" + enemyHealth.ToString();
        }

        plrHealth = Mathf.Clamp(plrHealth, 0, 50);
        enemyHealth = Mathf.Clamp(enemyHealth, 0, 50);
    }
    public void AssignCard(Card targetCard)
    {
        if (targetCard.IsPlayerCard)
        {
            TotalCardsPLayed++;
            if (targetCard.uniqueCard != Uniquecard.None && !CheckForBuff(targetCard, CardType.Spade, Uniquecard.Jack))
            {
                print(targetCard.IsUnityNull());
                print(PlayerBuffCard.IsUnityNull());
                PlayerBuffCard.Add(targetCard);

            } else if (targetCard.uniqueCard != Uniquecard.None && CheckForBuff(targetCard, CardType.Spade, Uniquecard.Jack))
            {
                PlayerHasExtraMove = true;
            }

            else
            {
                PlayerCard.Add(targetCard);
                if (PlayerHasExtraMove)
                {

                    PlayerHasExtraMove = false;
                }
                else
                {
                    StartCoroutine(Delay(0.2f, () => { CamAnimator.ToPos(true); }));
                    StartCoroutine(Delay(1f, () => { WaitForEval(); }));
                }
            }
            
        }
        else
        {
            if (targetCard.uniqueCard != Uniquecard.None)
            {
                EnemyBuffCard.Add(targetCard);
            }
            else
            {
                EnemyCard.Add(targetCard);
            }
        }
    }

    bool CheckForBuff(List<Card> target, CardType type, Uniquecard Unique)
    {
        foreach (Card buffCard in target)
        {
            if (buffCard.uniqueCard == Unique && buffCard.cardType == type)
            {
                return true;
            }
        }
        return false;
    }
    bool CheckForBuff(Card target, CardType type, Uniquecard Unique)
    {
        if(target.IsUnityNull())
        {
            return false;
        } else
        {
            if (target.uniqueCard == Unique && target.cardType == type)
            {
                return true;
            }
        }
        return false;
    }

    public void ClearEval()
    {

        SetEvalTexts(false, true);
        foreach(textshow fx in BuffVisuals)
        {
            fx.SetAppearance(false);
        }
        PlayerCard.Clear();
        EnemyCard.Clear();
        PlayerBuffCard.Clear();
        EnemyBuffCard.Clear();
        EnemyMoves = 1;
        EnemyCardsPlayed = 0;
        PlayerCardsPlayed = 0;
        CamAnimator.ToPos(false);
    }

    public void WaitForEval()
    {
        ReadDeckScript.SetCardActivity(false);
        EnemyScript.SelectCard(true, EnemyMoves);
    }

    IEnumerator Delay(float delayTime, Action callback)
    {
        yield return new WaitForSeconds(delayTime);
        callback();
    }

    int StackRawDamage(List<Card> cards)
    {
        int RawDamage = 0;
        foreach (Card card in cards)
        {
            RawDamage += card.Damage;
        }
        return RawDamage;
    }

    void CreateBuffEffect(string text, bool isPositive, bool isPlayerSide, bool isCenter = false)
    {
        GameObject effectMain = Instantiate(effectPreset, new Vector3(0, 0, 0), Quaternion.identity);
        BuffVisuals.Add(effectMain.GetComponent<textshow>());
        TMP_Text effectText = effectMain.GetComponent<TMP_Text>();
        string finalText;
        if (!isCenter)
        {
            if (isPlayerSide)
            {
                effectMain.transform.parent = playerSide;
            }
            else
            {
                effectMain.transform.parent = enemySide;
            }
            finalText = isPositive ? "+" + text : "-" + text;
        }
        else
        {
            effectMain.transform.parent = Center;
            finalText = text;
        }

            
        Util.TypeWrite(this, finalText, effectText, 0.05f);
    }

    List<Card> GetKings(List<Card> Reference)
    {
        List<Card> Kings = new();
        //For some reason putting a number in new() doesn't actually give the list 2 entries
        Kings.Add(null);
        Kings.Add(null);
        if(!Reference.IsUnityNull() && Reference.Count > 0)
        {
            foreach (Card card in Reference)
            {
                if (CheckForBuff(card, CardType.Spade, Uniquecard.King))
                {
                    Kings[0] = card;
                }
                else if (CheckForBuff(card, CardType.Heart, Uniquecard.King))
                {
                    Kings[1] = card;
                }
            }
        }
        return Kings;
    }

    public void EvalDamage()
    {
        StartCoroutine(EvalDamage_Anim());
    }

    public void SetText(bool Playerside, int targetNum, bool Center = false, bool StackEffect = false)
    {
        TMP_Text target;
        if (!Center)
        {
            if (Playerside)
            {
                target = PlayerDam;
            }
            else
            {
                target = EnemyDam;
            }
        } else
        {
            target = Total;
        }
        int currentNum = 0;
        
        foreach(char c in target.text)
        {
            currentNum += (int)char.GetNumericValue(c);
        }
        print("Comparing" + currentNum.ToString() + " to " + targetNum.ToString());
        bool EffectType = currentNum < targetNum ? true : false; //shows whether the effect is positive or negative
        if (!Center)
        {
            target.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            target.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            target.color = EffectType ? new Color(0.5f, 1, 0.5f) : new Color(1, 0.5f, 0.5f); //Green for positive red for negative
        } else
        {
            if (StackEffect) 
            {
                target.rectTransform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                target.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f).SetEase(Ease.OutBack);
            } else
            {
                target.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f).SetEase(Ease.OutBack);
            }
        }
        target.text = targetNum.ToString();
        target.DOColor(new Color(1, 1, 1), 0.5f);
    }

    public void SetText(TMP_Text target, int targetNum, bool Center = false)
    {
        int currentNum = 0;

        foreach (char c in target.text)
        {
            currentNum += (int)char.GetNumericValue(c);
        }
        print("Comparing" + currentNum.ToString() + " to " + targetNum.ToString());
        bool EffectType = currentNum < targetNum ? true : false; //shows whether the effect is positive or negative
        if (!Center)
        {
            target.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            target.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            target.color = EffectType ? new Color(0.5f, 1, 0.5f) : new Color(1, 0.5f, 0.5f); //Green for positive red for negative
        }
        else
        {
            target.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f).SetEase(Ease.OutBack);
        }
        target.text = targetNum.ToString();
        target.DOColor(new Color(1, 1, 1), 0.5f);
    }

    void SetEvalTexts(bool toggle, bool includeCenter = false)
    {
        if (toggle)
        {
            PlayerDam.rectTransform.localPosition = Vector3.zero;
            EnemyDam.rectTransform.localPosition = Vector3.zero;
            PlayerDam.rectTransform.DOLocalMoveX(-200f, 0.5f).SetEase(Ease.OutQuad);
            EnemyDam.rectTransform.DOLocalMoveX(200f, 0.5f).SetEase(Ease.OutQuad);
            PlayerDam.color = new Color(1, 1, 1, 0);
            EnemyDam.color = new Color(1, 1, 1, 0);
        } else
        {
            PlayerDam.rectTransform.DOLocalMoveX(0, 0.5f).SetEase(Ease.InQuad);
            EnemyDam.rectTransform.DOLocalMoveX(0, 0.5f).SetEase(Ease.InQuad);
        }
            PlayerDam.DOColor(toggle ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0), 0.5f).SetEase(Ease.InQuad);
        EnemyDam.DOColor(toggle ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0), 0.5f).SetEase(Ease.InQuad);
        if(includeCenter)
        {
            Total.rectTransform.localScale = Vector3.one;
            Total.text = "0";
            Total.DOColor(toggle ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0), 0.5f);
        }
    }

    public IEnumerator EvalDamage_Anim()
    {
        usedTurns++;
        bool TargetsPlayer = false;
        int InitialDamage = 0;
        int FinalDamage = 0;
        SetEvalTexts(true, true);
        int PlayerIntendedDamage = StackRawDamage(PlayerCard);
        int EnemyIntendedDamage = StackRawDamage(EnemyCard);
        PlayerDam.text = PlayerIntendedDamage.ToString();
        EnemyDam.text = EnemyIntendedDamage.ToString();
        yield return new WaitForSeconds(1f);
        List<Card> PlayerKings = new List<Card>(2);
        List<Card> EnemyKings = new List<Card>(2);
        PlayerKings = GetKings(PlayerBuffCard);
        EnemyKings = GetKings(EnemyBuffCard);



        int[] damage = CalculateTypingAdvantage(PlayerIntendedDamage, EnemyIntendedDamage, CheckForBuff(PlayerBuffCard, CardType.Club, Uniquecard.Ace) || CheckForBuff(EnemyBuffCard, CardType.Club, Uniquecard.Ace), CheckForBuff(PlayerBuffCard, CardType.Spade, Uniquecard.Queen) || CheckForBuff(EnemyBuffCard, CardType.Spade, Uniquecard.Queen));
        PlayerIntendedDamage = damage[0];
        EnemyIntendedDamage = damage[1];
        if (TempDebounce)
        {
            if (WaitExtra)
            {
                yield return new WaitForSeconds(2f);
                WaitExtra = false;
            } else
            {
                yield return def;
            }
                TempDebounce = false;
        }
        PlayerIntendedDamage = AffectIntended(PlayerKings[0], EnemyKings[1], PlayerIntendedDamage);
        if (TempDebounce)
        {
            yield return def;
            TempDebounce = false;
        }
        EnemyIntendedDamage = AffectIntended(EnemyKings[0], PlayerKings[1], EnemyIntendedDamage);
        if (TempDebounce)
        {
            yield return def;
            TempDebounce = false;
        }



        print(PlayerIntendedDamage.ToString() + " - " + EnemyIntendedDamage.ToString() + " = " + (PlayerIntendedDamage - EnemyIntendedDamage).ToString());
        if (TempDebounce)
        {
            yield return def;
            TempDebounce = false;
        }
        SetEvalTexts(false, false);
        InitialDamage = PlayerIntendedDamage - EnemyIntendedDamage;
        yield return new WaitForSeconds(0.4f);
        SetText(true, Mathf.Abs(InitialDamage), true);
        yield return def;


        try //Buff cards may trigger a null error on this so it's put in a try
        {
            if (CheckForBuff(PlayerBuffCard, CardType.Heart, Uniquecard.Jack) || CheckForBuff(EnemyBuffCard, CardType.Heart, Uniquecard.Jack))
            {
                InitialDamage = 0;
                CreateBuffEffect("Jack of hearts", false, true, true);
                SetText(true, 0, true);
                TempDebounce = true;
            }
            else if (CheckForBuff(PlayerBuffCard, CardType.Spade, Uniquecard.Ace) && InitialDamage < 0)
            {
                InitialDamage = -InitialDamage;
                CreateBuffEffect("Ace of spades", true, true, true);
                TempDebounce = true;
            }
            else if (CheckForBuff(EnemyBuffCard, CardType.Spade, Uniquecard.Ace) && InitialDamage > 0)
            {
                InitialDamage = -InitialDamage;
                CreateBuffEffect("Ace of spades", true, false, true);
                TempDebounce = true;
            }
        }
        catch { }
        if(TempDebounce)
        {
            yield return def;
            TempDebounce = false;
        }
        TargetsPlayer = InitialDamage < 0 ? true : false;
        if (TargetsPlayer)
        {
            Total.DOColor(new Color(1f, .5f, .5f, 1), 0.5f);
        } else
        {
            Total.DOColor(new Color(.5f, 1, .5f, 1), 0.5f);
        }
        yield return def;
        Total.DOFade(0,.5f);
        FinalDamage = Mathf.Abs(InitialDamage);

        if (TargetsPlayer)
        {
            plrHealth -= FinalDamage;
        }
        else
        {
            enemyHealth -= FinalDamage;
        }
        HealthVisualiserScript.UpdateDecks();

        yield return new WaitForSeconds(1f);
        //Save last move for Ace of Hearts
        LastDealtDamage = FinalDamage;
        TargetedPlayer = TargetsPlayer;
        WaitTilWin = false;
        FinishGame();
        if (WinCheck) //Either the player or the enemy has Won
        {
            gameEndScript.CardsUsed = TotalCardsPLayed;
            gameEndScript.SubmitResults();
            ReadDeckScript.StartCoroutine(ReadDeckScript.ItsJoever());
            ClearEval();
            GuideShowerScript.ShowGuideOption(false);
            List<string> dialog = new List<string>();
            if (Winside == true)
            {
                dialog.Add("Well, that's something I don't see often.");
                dialog.Add("You managed to win from me.");
                dialog.Add("I suppose I underestimated you.");
                dialog.Add("You can keep the chips you won from me.");
                dialog.Add("Come back if you ever want to have another match.");
                gameEndScript.WinLose.text = "You won double your chips!";
            }
            else
            {
                dialog.Add("Well, looks like you lost.");
                dialog.Add("It was a good match, I'll admit.");
                dialog.Add("Come back when you have another 50 chips to spare.");
                gameEndScript.WinLose.text = "You lost your chips.";
            }
            dialog.Add("I'll be waiting.");
            dialogSystem.DialogueSequence(dialog);
            yield return new WaitUntil(() => dialogSystem.DialogueFinished);
            gameEndScript.StartCoroutine(gameEndScript.FadeIn());

        } else {
            if (tutlog.isFirstEverTurn && tutlog.IncludeTutorial)
            {
                ClearEval();
                List<string> dialog = new List<string>();
                dialog.Add("Something I forgot to mention.");
                dialog.Add("Cards can have winning suits, adding a 1.5x boost to the value.");
                dialog.Add("Hearts beat Clubs,");
                dialog.Add("Clubs beat Diamonds,");
                dialog.Add("Diamonds beat Spades,");
                dialog.Add("and Spades beat Hearts.");
                dialog.Add("Of course, I won't just show you my cards, so try getting lucky.");
                dialogSystem.DialogueSequence(dialog);
                yield return new WaitUntil(() => dialogSystem.DialogueFinished);
                tutlog.isFirstEverTurn = false;
                tutlog.IncludeTutorial = false;
            }
            if (usedTurns >= TurnsUntilFace)
            {
                usedTurns = 0;
                ReadDeckScript.Regen(true);
            }
            else
            {
                ReadDeckScript.Regen();
            }

            ClearEval();
        }
    }

    void FinishGame()
    {
        if(plrHealth <= 0 && enemyHealth > 0) //Player lost
        {
            WinCheck = true;
            Winside = false;
            return;
        }
        if (plrHealth > 0 && enemyHealth <= 0) //Player won
        {
            WinCheck = true;
            Winside = true;
            return;
        }
    }

    public int AffectIntended(Card card1, Card card2, int NumberModify)
    {
        bool IntendedGoesTo;
        int ReturnDamage;
        if(card1.IsUnityNull() && card2.IsUnityNull())
        {
            return NumberModify;
        } else if (card1.IsUnityNull())
        {
            IntendedGoesTo = !card2.IsPlayerCard;
        }
        else if (card2.IsUnityNull())
        {
            IntendedGoesTo = card1.IsPlayerCard;
        } else
        {
            IntendedGoesTo = true;
            Debug.LogError("AffectIndended is fucking killing itself brother");
        }
        if (!card1.IsUnityNull() && card1.uniqueCard == Uniquecard.King && card1.cardType == CardType.Spade)
        {
            ReturnDamage = NumberModify + 3;
            CreateBuffEffect("king of spades", true, IntendedGoesTo);
            SetText(IntendedGoesTo, ReturnDamage);
            TempDebounce = true;
        }
        else
        {
            ReturnDamage = NumberModify;
        }
        int DamagePostBuff = ReturnDamage;
        if (!card2.IsUnityNull() && card2.uniqueCard == Uniquecard.King && card2.cardType == CardType.Heart)
        {
            DamagePostBuff = NumberModify - 3;
            CreateBuffEffect("king of Hearts", false, IntendedGoesTo);
            SetText(IntendedGoesTo, DamagePostBuff);
            TempDebounce = true;
        }
        else
        {
            ReturnDamage = NumberModify;
        }
        int NewDamage = DamagePostBuff;
        NewDamage = Mathf.Clamp(NewDamage, 0, 50);
        return NewDamage;
    }

    /*
    Hearts beats Clubs
    Clubs beats Diamonds
    Diamonds beat Spades
    Spades beat Hearts
    Hearts > Clubs > Diamonds > Spades > Hearts
    */


    private int[] CalculateTypingAdvantage(int playerDamage, int enemyDamage, bool aceOfClubs, bool queenOfSpades)
    {
        int typingAdvantage = 0;
        // positive favors player negative favors enemy
        if(PlayerCard.Count > 1 || EnemyCard.Count > 1)
        {
            Debug.LogError("Can't calculate typing advantage. More than 1 normal card present.");
            return new int[] { playerDamage, enemyDamage };
        }
        foreach (Card playerCard in PlayerCard)
        {
            int playerCardType = Util.FindIndexOfItemInArray(typeChart, playerCard.name[1]);
            foreach (Card enemyCard in EnemyCard)
            {
                int enemyCardType = Util.FindIndexOfItemInArray(typeChart, enemyCard.name[1]);
                if (enemyCardType != playerCardType)
                {
                    if (enemyCardType == playerCardType - 1 || (enemyCardType == 3 && playerCardType == 0))
                        typingAdvantage--;
                    else if (enemyCardType == playerCardType + 1 || (enemyCardType == 0 && playerCardType == 3))
                        typingAdvantage++;
                }
            }
        }
        if (aceOfClubs)
            { typingAdvantage = -typingAdvantage; }
        if (typingAdvantage > 0)
        {
            CreateBuffEffect("Suit bonus", true, true);
            print("Typing favors player");
            if (tutlog.isFirstEverTurn && tutlog.IncludeTutorial)
            {
                dialogSystem.SingleDialog("A suit bonus on your first move, not bad.");
                WaitExtra = true;
            }
            playerDamage += Mathf.RoundToInt(playerDamage *(queenOfSpades ? 1 : .5f));
            SetText(PlayerDam, playerDamage);
            TempDebounce = true;
        }
        else if (typingAdvantage < 0)
        {
            print("Typing favors enemy");
            CreateBuffEffect("Suit bonus", true, false);
            enemyDamage += Mathf.RoundToInt(enemyDamage * (queenOfSpades ? 1 : .5f));
            SetText(EnemyDam, enemyDamage);
            TempDebounce = true;
        }
        else
            print("Typing favors no-one");

        return new int[] { playerDamage, enemyDamage };
    }
}
