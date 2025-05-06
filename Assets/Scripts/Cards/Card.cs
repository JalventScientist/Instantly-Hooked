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
        cardText.text = cardType.ToString() + " " + Damage;
    }

    public void ApplyCard()
    {
        EvaluateDamage evaluateDamage = FindFirstObjectByType<EvaluateDamage>();
        if (evaluateDamage != null)
        {
            evaluateDamage.AssignCard(IsPlayerCard, this);
        }
        else
        {
            Debug.LogError("EvaluateDamage component not found in the scene.");
        }
    }
}
