using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

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


    //Card Matching
    bool isTied = false;
    bool PlayerSuit = false;

    public bool UsedCard = false;

    private void Awake()
    {
        ButtonTransform = transform.GetChild(0).GetComponent<RectTransform>();
        hoverTriggers = ButtonTransform.GetComponent<EventTrigger>();
    }

    public virtual void ApplyCard()
    {
        if (AlreadySetup)
        {
            hoverTriggers.enabled = false;
            EvaluateDamage evaluateDamage = FindFirstObjectByType<EvaluateDamage>();
            GameObject throwCard = Instantiate(Resources.Load<GameObject>("Prefabs/Cards/ThrownCard"), new Vector3(IsPlayerCard ? -10 : 10, 0, 0), Quaternion.identity);
            throwCard.GetComponent<ThrowCard>().RenderProperCard(transform.name, IsPlayerCard);
            ButtonTransform.GetComponent<Button>().enabled = false;
            ButtonTransform.DOLocalMoveY(-500f,0.3f).SetEase(Ease.InOutQuad);
            UsedCard = true;
             if (evaluateDamage != null && uniqueCard == Uniquecard.None)
            {
                evaluateDamage.AssignCard(this);
            }
            else
            {
                Debug.LogError("EvaluateDamage component not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("Cards not setup yet. run SetupCard() when loading the card prefab.");
        }
    }

    public void SetupCard()
    {
        cardTransform = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        cardRender = cardTransform.GetComponent<Image>();
        cardRender.sprite = Resources.Load<Sprite>("CardImages/" + transform.name);
        AlreadySetup = true;
        if (!IsPlayerCard)
        {
            ButtonTransform.GetComponent<Button>().enabled = false;
        }

        List<string> cardData = new List<string>();

        foreach(char c in transform.name)
        {
            cardData.Add(c.ToString());
        }

        switch (cardData[0])
        {
            case "A":
                uniqueCard = Uniquecard.Ace;
                break;
            case "K":
                uniqueCard = Uniquecard.King;
                break;
            case "Q":
                uniqueCard = Uniquecard.Queen;
                break;
            case "J":
                uniqueCard = Uniquecard.Jack;
                break;
            default:
                uniqueCard = Uniquecard.None;
                
                int Intended = int.Parse(cardData[0]);
                if(Intended == 0)
                {
                    Intended = 10;
                }
                Damage = Intended;
                break;
        }
        switch (cardData[1])
        {
            case "H":
                cardType = CardType.Heart;
                break;
            case "S":
                cardType = CardType.Spade;
                break;
            case "D":
                cardType = CardType.Diamond;
                break;
            case "C":
                cardType = CardType.Club;
                break;
            default:
                Debug.LogError("Card Type not found");
                break;
        }
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
