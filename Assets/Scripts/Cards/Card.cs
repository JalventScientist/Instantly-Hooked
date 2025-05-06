using TMPro;
using UnityEngine;

public enum Uniquecard{
    Ace,
    King,
    Queen,
    Jack,
    None
}
public class Card : MonoBehaviour
{
    public CardType cardType;
    public int Damage;
    public bool IsPlayerCard;
    public Uniquecard uniqueCard;


    [SerializeField] TMP_Text cardText;

    private void Start()
    {
        if(uniqueCard != Uniquecard.None)
        {
            cardText.text = cardType.ToString() + " " + uniqueCard.ToString();
        }
        else
        {
            cardText.text = cardType.ToString() + " " + Damage;
        }
    }

    public virtual void ApplyCard()
    {
        EvaluateDamage evaluateDamage = FindFirstObjectByType<EvaluateDamage>();
        if (evaluateDamage != null)
        {
            evaluateDamage.AssignCard(this);
        }
        else
        {
            Debug.LogError("EvaluateDamage component not found in the scene.");
        }
    }
}
