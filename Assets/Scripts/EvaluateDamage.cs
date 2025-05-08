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

    int plrHealth = 50;
    int enemyHealth = 50;

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
                PlayerBuffCard = targetCard;
            }
            else
            {
                PlayerCard.Add(targetCard);
            }
            WaitForEval();
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

        int PlayerIntendedDamage = StackRawDamage(PlayerCard);
        int EnemyIntendedDamage = StackRawDamage(EnemyCard);

        //Add changes pre-attack if using Heart/Spade King
        PlayerIntendedDamage = AffectIntended(PlayerBuffCard, EnemyBuffCard, PlayerIntendedDamage);
        EnemyIntendedDamage = AffectIntended(EnemyBuffCard, PlayerBuffCard, EnemyIntendedDamage);

        InitialDamage = PlayerIntendedDamage - EnemyIntendedDamage;
        if((!PlayerBuffCard.IsUnityNull() && PlayerBuffCard.uniqueCard == Uniquecard.Jack && PlayerBuffCard.cardType == CardType.Heart) && (!EnemyBuffCard.IsUnityNull() && PlayerBuffCard.uniqueCard == Uniquecard.Jack && EnemyBuffCard.cardType == CardType.Heart))
        {
            InitialDamage = 0;
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
    }

    public int AffectIntended(Card card1, Card card2, int NumberModify)
    {
        int ReturnDamage;
        ReturnDamage = !card1.IsUnityNull() && card1.uniqueCard == Uniquecard.King && card1.cardType == CardType.Spade ?
            NumberModify + 3 : NumberModify;
        print(!card2.IsUnityNull() && card2.uniqueCard == Uniquecard.King && card2.cardType == CardType.Heart);
        ReturnDamage = !card2.IsUnityNull() && card2.uniqueCard == Uniquecard.King && card2.cardType == CardType.Heart ?
    NumberModify - 3 : ReturnDamage;

        return ReturnDamage;
    }
}
