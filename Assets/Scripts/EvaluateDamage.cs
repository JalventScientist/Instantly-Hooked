using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;

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

    Card PlayerBuffCard = null;
    Card EnemyBuffCard = null;

    BasicEnemy EnemyScript;
    ReadDeck ReadDeckScript;

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

    private void Start()
    {
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
                PlayerBuffCard = targetCard;

            }
            else
            {
                PlayerCard.Add(targetCard);
                try
                {
                    if (!PlayerBuffCard.IsUnityNull() && PlayerBuffCard.uniqueCard == Uniquecard.Jack && PlayerBuffCard.cardType == CardType.Spade)
                    {
                        PlayerBuffCard = null;
                    }
                    else
                    {
                        WaitForEval();
                    }
                }
                catch
                {
                    WaitForEval();
                }
            }
            
        }
        else
        {
            if (targetCard.uniqueCard != Uniquecard.None)
            {
                EnemyBuffCard = targetCard;
            }
            else
            {
                EnemyCard.Add(targetCard);
            }
        }
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
    
    int StackRawDamage(List<Card> cards)
    {
        int RawDamage = 0;
        foreach (Card card in cards)
        {
            RawDamage += card.Damage;
        }
        return RawDamage;
    }
    
    public void EvalDamage()
    {
        bool TargetsPlayer = false;
        int InitialDamage = 0;
        int FinalDamage = 0;

        print(EnemyCard.Count);
        int PlayerIntendedDamage = StackRawDamage(PlayerCard);
        int EnemyIntendedDamage = StackRawDamage(EnemyCard);

        //Add changes pre-attack if using Heart/Spade King
        PlayerIntendedDamage = AffectIntended(PlayerBuffCard, EnemyBuffCard, PlayerIntendedDamage);
        EnemyIntendedDamage = AffectIntended(EnemyBuffCard, PlayerBuffCard, EnemyIntendedDamage);

        CalculateTypingAdvantage(PlayerIntendedDamage, EnemyIntendedDamage, (PlayerBuffCard.uniqueCard == Uniquecard.Ace && PlayerBuffCard.cardType == CardType.Club) || (EnemyBuffCard.uniqueCard == Uniquecard.Ace && EnemyBuffCard.cardType == CardType.Club), (PlayerBuffCard.uniqueCard == Uniquecard.Queen && PlayerBuffCard.cardType == CardType.Spade) || (EnemyBuffCard.uniqueCard == Uniquecard.Queen && EnemyBuffCard.cardType == CardType.Spade));

        print(PlayerIntendedDamage.ToString() + " - " + EnemyIntendedDamage.ToString() + " = " + (PlayerIntendedDamage - EnemyIntendedDamage).ToString());

        InitialDamage = PlayerIntendedDamage - EnemyIntendedDamage;


        try //Buff cards may trigger a null error on this so it's put in a try
        {
            if ((!PlayerBuffCard.IsUnityNull() && PlayerBuffCard.uniqueCard == Uniquecard.Jack && PlayerBuffCard.cardType == CardType.Heart) || (!EnemyBuffCard.IsUnityNull() && EnemyBuffCard.uniqueCard == Uniquecard.Jack && EnemyBuffCard.cardType == CardType.Heart))
            {
                InitialDamage = 0;
            }
            else if ((!PlayerBuffCard.IsUnityNull() && PlayerBuffCard.uniqueCard == Uniquecard.Ace && PlayerBuffCard.cardType == CardType.Spade) && InitialDamage < 0)
            {
                InitialDamage = -InitialDamage;
            }
            else if ((!EnemyBuffCard.IsUnityNull() && EnemyBuffCard.uniqueCard == Uniquecard.Ace && EnemyBuffCard.cardType == CardType.Spade) && InitialDamage > 0)
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
        print(NumberModify);
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

    private char[] typeChart = { 'H', 'C', 'D', 'S' };
    private void CalculateTypingAdvantage(int playerDamage, int enemyDamage, bool aceOfClubs, bool queenOfSpades)
    {
        int typingAdvantage = 0;
        // positive favors player negative favors enemy
        foreach (Card playerCard in PlayerCard)
        {
            int playerCardType = Util.FindIndexOfItemInArray(typeChart, playerCard.name[1]);
            if (playerCardType == -1)
            {
                Debug.LogError("WrongPartOfTheCardName");
                return;
            }
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
            typingAdvantage = -typingAdvantage;
        if (typingAdvantage > 0)
        {
            print("Typing favors player");
            playerDamage += extraDamageFromTypingAdvantage * (queenOfSpades ? 1 : 2);
        }
        else if (typingAdvantage < 0)
        {
            print("Typing favors enemy");
            enemyDamage += extraDamageFromTypingAdvantage * (queenOfSpades ? 1 : 2);
        }
        else
            print("Typing favors no-one");
    }
}
