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

    public int plrHealth = 50;
    public int enemyHealth = 50;

    public bool PlayerHasExtraMove = false;
    public bool EnemyHasExtraMove = false;

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
    }
    public void AssignCard(Card targetCard)
    {
        if (targetCard.IsPlayerCard)
        {
            if (targetCard.uniqueCard != Uniquecard.None)
            {
                print("PlayerCard Buff");
                PlayerBuffCard = targetCard;

            }
            else
            {
                print("PlayerCard Normal");
                PlayerCard.Add(targetCard);
                WaitForEval();
            }
            
        }
        else
        {
            if (targetCard.uniqueCard != Uniquecard.None)
            {
                print("EnemyCard Buff");
                EnemyBuffCard = targetCard;
            }
            else
            {
                print("EnemyCard Normal");
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
    }

    public void WaitForEval()
    {
        ReadDeckScript.SetCardActivity(false);
        EnemyScript.SelectCard();
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

        print(PlayerIntendedDamage.ToString() + " - " + EnemyIntendedDamage.ToString() + " = " + (PlayerIntendedDamage - EnemyIntendedDamage).ToString());

        InitialDamage = PlayerIntendedDamage - EnemyIntendedDamage;


        try
        {
            if ((!PlayerBuffCard.IsUnityNull() && PlayerBuffCard.uniqueCard == Uniquecard.Jack && PlayerBuffCard.cardType == CardType.Heart) || (!EnemyBuffCard.IsUnityNull() && PlayerBuffCard.uniqueCard == Uniquecard.Jack && EnemyBuffCard.cardType == CardType.Heart))
            {
                InitialDamage = 0;
                print("Jaque of hearts applied");
            }
        }
        catch
        {
            print("Le fragile occurred lmao");
        }

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
            print("Original Damage:" + NumberModify + " | New Damage" + ReturnDamage);
        } else
        {
            ReturnDamage = NumberModify;
        }
        int DamagePostBuff = ReturnDamage;
        if (!card2.IsUnityNull() && card2.uniqueCard == Uniquecard.King && card2.cardType == CardType.Heart)
        {
            DamagePostBuff = NumberModify - 3;
            print("King Heart buff applied");
        }
        else
        {
            ReturnDamage = NumberModify;
        }
        int NewDamage = DamagePostBuff;
        return NewDamage;
    }
}
