using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [Range(2,10)]public int Damage;
    public bool IsPlayerCard;
    public Uniquecard uniqueCard;


    Image cardRender;
    RectTransform cardTransform;
    RectTransform ButtonTransform;

    private void Awake()
    {
        ButtonTransform = GetComponent<RectTransform>();

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

    public void SetupCard()
    {
        cardTransform = transform.GetChild(0).GetComponent<RectTransform>();
        cardRender = cardTransform.GetComponent<Image>();
        cardRender.sprite = Resources.Load<Sprite>("CardImages/" + transform.name);
        print("a");
    }
}
