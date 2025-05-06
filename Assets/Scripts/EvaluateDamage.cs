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

    public void AssignCard(bool IsPlayer, Card targetCard)
    {
        if (!IsPlayer)
        {
            
        }
    }

    public void EvalDamage()
    {
        int InitialDamage = 0;
        int FinalDamage = 0;

        int PlayerIntendedDamage = 0;
        int EnemyIntendedDamage = 0;

        //Add changes pre-attack if using Heart/Spade King
        PlayerIntendedDamage = AffectIntended(PlayerBuffCard, EnemyBuffCard, PlayerIntendedDamage);
        EnemyIntendedDamage = AffectIntended(EnemyBuffCard, PlayerBuffCard, EnemyIntendedDamage);

        InitialDamage = PlayerIntendedDamage - EnemyIntendedDamage;
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
