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

    Card EnemyCard;
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
        float InitialDamage = 0;
        float FinalDamage = 0;

        float PlayerIntendedDamage = 0;
        float EnemyIntendedDamage = 0;

        
    }

    public int AffectIntended(Card card, int NumberModify)
    {
        int ReturnDamage;
        if (card != null && card.uniqueCard != Uniquecard.None)
        {
            switch (card.uniqueCard)
            {
                case Uniquecard.Ace:

                    break;
                case Uniquecard.King:

                    break;
                case Uniquecard.Queen:

                    break;
                case Uniquecard.Jack:

                    break;
                default:
                    return NumberModify;
            }
        } else
        {
            return NumberModify;
        }
    }
}
