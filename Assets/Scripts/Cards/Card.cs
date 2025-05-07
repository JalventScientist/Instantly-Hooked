using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    bool Hovering = false;
    bool AlreadySetup = false;

    Image cardRender;
    RectTransform cardTransform;
    RectTransform ButtonTransform;

    EventTrigger hoverTriggers; //Has to be disabled once clicked
    private void Awake()
    {
        ButtonTransform = transform.GetChild(0).GetComponent<RectTransform>();
        hoverTriggers = ButtonTransform.GetComponent<EventTrigger>();
    }

    public virtual void ApplyCard()
    {
        hoverTriggers.enabled = false;
        EvaluateDamage evaluateDamage = FindFirstObjectByType<EvaluateDamage>();
        GameObject throwCard = Instantiate(Resources.Load<GameObject>("Prefabs/Cards/ThrownCard"), new Vector3(IsPlayerCard ? -10 : 10,0,0), Quaternion.identity);
        throwCard.GetComponent<ThrowCard>().RenderProperCard(transform.name);
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
        cardTransform = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        cardRender = cardTransform.GetComponent<Image>();
        cardRender.sprite = Resources.Load<Sprite>("CardImages/" + transform.name);
        AlreadySetup = true;
    }

    public void SetHover(bool toggle)
    {
        if (IsPlayerCard)
        {
            Hovering = toggle;
        }
    }

    private void Update()
    {
        if (AlreadySetup)
        {

            if (!Hovering)
            {
                //379.5
                ButtonTransform.DOSizeDelta(new Vector2(0, 379.5f), 0.3f);
                cardTransform.DOLocalMoveY(0, 0.3f);
            }
            else
            {
                ButtonTransform.DOSizeDelta(new Vector2(0, 501f), 0.3f);
                cardTransform.DOLocalMoveY(60.3f, 0.3f);
            }
        }
    }
}
