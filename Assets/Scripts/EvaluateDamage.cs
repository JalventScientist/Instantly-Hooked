using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using TMPro;
using System;

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

    private char[] typeChart = { 'H', 'C', 'D', 'S' };

    private void Start()
    {
        CamAnimator = FindFirstObjectByType<AnimateCam>();
        EnemyScript = GetComponent<BasicEnemy>();
        ReadDeckScript = GetComponent<ReadDeck>();
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
            if (targetCard.uniqueCard != Uniquecard.None)
            {
                PlayerBuffCard.Add(targetCard);

            }
            else
            {
                PlayerCard.Add(targetCard);
                try
                {
                    if (CheckForBuff(PlayerBuffCard, CardType.Spade, Uniquecard.Jack))
                    {
                        PlayerBuffCard = null;
                    }
                    else
                    {
                        StartCoroutine(Delay(0.2f, () => { CamAnimator.ToPos(true); }));
                        StartCoroutine(Delay(1f, () => { WaitForEval(); }));
                    }
                }
                catch
                {
                    StartCoroutine(Delay(0.2f, () => { CamAnimator.ToPos(true); }));
                    StartCoroutine(Delay(0.3f, () => { WaitForEval(); }));
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
        PlayerCard.Clear();
        EnemyCard.Clear();
        PlayerBuffCard = null;
        EnemyBuffCard = null;
        EnemyMoves = 1;
        EnemyCardsPlayed = 0;
        PlayerCardsPlayed = 0;
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
    
    public IEnumerator EvalDamage_Anim()
    {
        bool TargetsPlayer = false;
        int InitialDamage = 0;
        int FinalDamage = 0;

        yield return new WaitForSeconds(0.5f);
        int PlayerIntendedDamage = StackRawDamage(PlayerCard);
        int EnemyIntendedDamage = StackRawDamage(EnemyCard);
    }

    void ShowChangedNumbers()
    {

    }

    void CreateBuffEffect()
    {

    }

    List<Card> GetKings(List<Card> Reference)
    {
        List<Card> Kings = new(2);
        foreach(Card card in Reference)
        {
            if (CheckForBuff(card, CardType.Spade, Uniquecard.King))
            {
                Kings.Insert(0, card);
            } else if(CheckForBuff(card, CardType.Heart, Uniquecard.King))
            {
                Kings.Insert(1, card);
            }
        }
        return Kings;
    }

    public void EvalDamage()
    {
        bool TargetsPlayer = false;
        int InitialDamage = 0;
        int FinalDamage = 0;

        int PlayerIntendedDamage = StackRawDamage(PlayerCard);
        int EnemyIntendedDamage = StackRawDamage(EnemyCard);

        List<Card> PlayerKings = new List<Card>();
        List<Card> EnemyKings = new List<Card>();
        PlayerKings = GetKings(PlayerBuffCard);
        EnemyKings = GetKings(EnemyBuffCard);

        PlayerIntendedDamage = AffectIntended(PlayerKings[0], EnemyKings[1], PlayerIntendedDamage);
        EnemyIntendedDamage = AffectIntended(EnemyKings[0], PlayerKings[1], EnemyIntendedDamage);

        int[] damage = CalculateTypingAdvantage(PlayerIntendedDamage, EnemyIntendedDamage, CheckForBuff(PlayerBuffCard, CardType.Club, Uniquecard.Ace) || CheckForBuff(EnemyBuffCard, CardType.Club, Uniquecard.Ace), CheckForBuff(PlayerBuffCard, CardType.Spade, Uniquecard.Queen) || CheckForBuff(EnemyBuffCard, CardType.Spade, Uniquecard.Queen));
        PlayerIntendedDamage = damage[0];
        EnemyIntendedDamage = damage[1];

        print(PlayerIntendedDamage.ToString() + " - " + EnemyIntendedDamage.ToString() + " = " + (PlayerIntendedDamage - EnemyIntendedDamage).ToString());

        InitialDamage = PlayerIntendedDamage - EnemyIntendedDamage;


        try //Buff cards may trigger a null error on this so it's put in a try
        {
            if (CheckForBuff(PlayerBuffCard, CardType.Heart, Uniquecard.Jack) || CheckForBuff(EnemyBuffCard, CardType.Heart, Uniquecard.Jack))
            {
                InitialDamage = 0;
            }
            else if (CheckForBuff(PlayerBuffCard, CardType.Spade, Uniquecard.Ace) && InitialDamage < 0)
            {
                InitialDamage = -InitialDamage;
            }
            else if (CheckForBuff(EnemyBuffCard, CardType.Spade, Uniquecard.Ace) && InitialDamage > 0)
            {
                InitialDamage = -InitialDamage;
            }
        }
        catch { }

        TargetsPlayer = InitialDamage < 0 ? true : false;
        FinalDamage = Mathf.Abs(InitialDamage);
        if (TargetsPlayer)
        {
            plrHealth -= FinalDamage;
        }
        else
        {
            enemyHealth -= FinalDamage;
        }
        //Save last move for Ace of Hearts
        LastDealtDamage = FinalDamage;
        TargetedPlayer = TargetsPlayer;
        ReadDeckScript.Regen();
        ClearEval();
    }

    public int AffectIntended(Card card1, Card card2, int NumberModify)
    {
        
        int ReturnDamage;
        if(!card1.IsUnityNull() && card1.uniqueCard == Uniquecard.King && card1.cardType == CardType.Spade)
        {
            ReturnDamage = NumberModify + 3;
        } else
        {
            ReturnDamage = NumberModify;
        }
        int DamagePostBuff = ReturnDamage;
        if (!card2.IsUnityNull() && card2.uniqueCard == Uniquecard.King && card2.cardType == CardType.Heart)
        {
            DamagePostBuff = NumberModify - 3;
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
            print("Typing favors player");
            playerDamage += Mathf.RoundToInt(playerDamage *(queenOfSpades ? 1 : .5f));
        }
        else if (typingAdvantage < 0)
        {
            print("Typing favors enemy");
            enemyDamage += Mathf.RoundToInt(enemyDamage * (queenOfSpades ? 1 : .5f));
        }
        else
            print("Typing favors no-one");

        return new int[] { playerDamage, enemyDamage };
    }
}
