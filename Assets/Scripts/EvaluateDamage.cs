using UnityEngine;
using System.Collections.Generic;

public enum CardType
{
    Heart,
    Spade,
    Diamond,
    Club
}
public class EvaluateDamage : MonoBehaviour
{
    List<Card> EnemyCard;
    List<Card> PlayerCard;

    Card PlayerBuffCard;
    Card EnemyBuffCard;

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

    public void EvalDamage()
    {
        bool TargetsPlayer = false;
        int InitialDamage = 0;
        int FinalDamage = 0;

        int PlayerIntendedDamage = 0;
        int EnemyIntendedDamage = 0;

        //Add changes pre-attack if using Heart/Spade King
        PlayerIntendedDamage = AffectIntended(PlayerBuffCard, EnemyBuffCard, PlayerIntendedDamage);
        EnemyIntendedDamage = AffectIntended(EnemyBuffCard, PlayerBuffCard, EnemyIntendedDamage);

        InitialDamage = PlayerIntendedDamage - EnemyIntendedDamage;
        if((PlayerBuffCard != null && PlayerBuffCard.uniqueCard == Uniquecard.Jack && PlayerBuffCard.cardType == CardType.Heart) && (EnemyBuffCard != null && PlayerBuffCard.uniqueCard == Uniquecard.Jack && EnemyBuffCard.cardType == CardType.Heart))
        {
            InitialDamage = 0;
        }

        TargetsPlayer = InitialDamage < 0 ? true : false;
        FinalDamage = Mathf.Abs(InitialDamage);
    }

    public int AffectIntended(Card card1, Card card2, int NumberModify)
    {
        int ReturnDamage;

        ReturnDamage = card1 != null & card1.uniqueCard == Uniquecard.King && card1.cardType == CardType.Spade ?
            NumberModify + 3 : NumberModify;
        ReturnDamage -= card2 != null & card2.uniqueCard == Uniquecard.King && card2.cardType == CardType.Heart ?
    NumberModify - 3 : NumberModify;

        return ReturnDamage;
    }
}
