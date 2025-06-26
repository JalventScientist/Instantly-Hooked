using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

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
    public bool AlreadySetup = false;

    Image cardRender;
    RectTransform cardTransform;
    RectTransform ButtonTransform;

    EventTrigger hoverTriggers; //Has to be disabled once clicked

    public bool isSpecial = false;

    AudioSource cardAudioSource; //for the pulling sound

    //Card Matching
    bool isTied = false;
    bool PlayerSuit = false;

    public bool UsedCard = false;
    public bool TrulyActive = false;
    bool Turning = false;

    [HideInInspector] public bool WillBeAssigned = true;

    [HideInInspector] public TMP_Text InfoText;

    [Tooltip("Only used by special cards. Displays what the effect does.")]
    public string EffectText = "I do special stuff";

    //Waitng Times;
    WaitForSeconds SpinTime = new WaitForSeconds(0.25f);

    private void Awake()
    {
        if(TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            cardAudioSource = audioSource;
        }
        else
        {
            gameObject.AddComponent<AudioSource>();
            cardAudioSource = GetComponent<AudioSource>();
        }
        cardAudioSource.clip = Resources.Load<AudioClip>("card2");
        cardAudioSource.playOnAwake = false;
        cardAudioSource.Stop();
        ButtonTransform = transform.GetChild(0).GetComponent<RectTransform>();
        hoverTriggers = ButtonTransform.GetComponent<EventTrigger>();
        ButtonTransform.position = new Vector3(0, -400, 0);
        ButtonTransform.DOLocalMoveY(0, 0.3f).SetEase(Ease.OutBack);
        cardAudioSource.Play();
        if (isSpecial)
        {
            InfoText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            InfoText.text = EffectText;
        }
    }

    public void ForceRemoveCards() //Hide cards for reshuffle
    {
        TrulyActive = false;
        ButtonTransform.GetComponent<Button>().enabled = false;
        ButtonTransform.DOLocalMoveY(-700f, 0.3f).SetEase(Ease.InBack).OnComplete(()=> Destroy(gameObject));
    }

    public virtual void ApplyCard()
    {
        if (AlreadySetup && (TrulyActive || !IsPlayerCard))
        {
            hoverTriggers.enabled = false;
            EvaluateDamage evaluateDamage = FindFirstObjectByType<EvaluateDamage>();
            if (!IsPlayerCard)
            {
                evaluateDamage.EnemyCardsPlayed++;
            } else
            {
                evaluateDamage.PlayerCardsPlayed++;


            }
            float Offset = -1.494f + (IsPlayerCard ? (float)(evaluateDamage.PlayerCardsPlayed -1) / 20 : (float)(evaluateDamage.EnemyCardsPlayed-1) / 20);
                GameObject throwCard = Instantiate(Resources.Load<GameObject>("Prefabs/Cards/ThrownCard"), new Vector3(0, Offset, IsPlayerCard ? -5 : 5), Quaternion.identity);
            throwCard.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
            throwCard.GetComponent<ThrowCard>().RenderProperCard(transform.name, IsPlayerCard);
            ButtonTransform.GetComponent<Button>().enabled = false;
            ButtonTransform.DOLocalMoveY(-700f,0.3f).SetEase(Ease.InOutQuad);
            UsedCard = true;
            TrulyActive = false;
            if (evaluateDamage != null && WillBeAssigned)
            {
                evaluateDamage.AssignCard(this);
            }
        }
    }

    IEnumerator AnimateVisualizedCard(bool toggle)
    {
        if (toggle)
        {
            
            cardTransform.DOLocalRotate(new Vector3(0, -90, 0), 0.25f).SetEase(Ease.InSine);
            yield return SpinTime;
            if (!IsPlayerCard)
            {
                LoadCardTexture(true);
            }
            else
            {
                LoadCardTexture(false);
            }
            cardTransform.DOLocalRotate(new Vector3(0, -180, 0), 0.25f).SetEase(Ease.OutSine);
        } else
        {
            cardTransform.DOLocalRotate(new Vector3(0, -90, 0), 0.25f).SetEase(Ease.InSine);
            yield return SpinTime;
            if (!IsPlayerCard)
            {
                LoadCardTexture(false);
            }
            else
            {
                LoadCardTexture(true);
            }
            cardTransform.DOLocalRotate(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.OutSine);
        }
        yield return SpinTime;
        Turning = toggle;
        if (!toggle)
        {
            TrulyActive = true;
            hoverTriggers.enabled = true;
        }
    }
    public void TurnCard(bool show)
    {
        hoverTriggers.enabled = false;
        TrulyActive = false;
        StartCoroutine(AnimateVisualizedCard(show));
    }

    void LoadCardTexture(bool loadName)
    {
        if (loadName)
        {
            cardRender.sprite = Resources.Load<Sprite>("CardImages/" + transform.name);
        }
        else
        {
            cardRender.sprite = Resources.Load<Sprite>("CardImages/Back");
        }
    }

    public void SetupCard()
    {
        cardTransform = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        cardRender = cardTransform.GetComponent<Image>();
        LoadCardTexture(IsPlayerCard);
        if (!IsPlayerCard)
        {
            ButtonTransform.GetComponent<Button>().enabled = false;
        }
            
        AlreadySetup = true;

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
        if (AlreadySetup && TrulyActive)
        {

            if (!Hovering)
            {
                //379.5
                ButtonTransform.DOSizeDelta(new Vector2(0, 379.5f), 0.3f);
                cardTransform.DOLocalMoveY(0, 0.3f);
                if (isSpecial)
                {
                    InfoText.DOColor(new Color(1, 1, 1, 0), 0.3f);
                }
            }
            else
            {
                ButtonTransform.DOSizeDelta(new Vector2(0, 501f), 0.3f);
                cardTransform.DOLocalMoveY(60.3f, 0.3f);
                if (isSpecial)
                {
                    InfoText.DOColor(new Color(1, 1, 1, 1), 0.3f);
                }
            }
        } else if ((!AlreadySetup && !Turning) && !Turning)
        {
            cardTransform.DOLocalMoveY(-60f, 0.3f);
            InfoText.DOColor(new Color(1, 1, 1, 1), 0.3f);
            if (isSpecial)
            {
                InfoText.DOColor(new Color(1, 1, 1, 0), 0.3f);
            }
        } else
        {
            ButtonTransform.DOSizeDelta(new Vector2(0, 379.5f), 0.3f);
            cardTransform.DOLocalMoveY(0, 0.3f);
            if (isSpecial)
            {
                InfoText.DOColor(new Color(1, 1, 1, 0), 0.3f);
            }
        }
    }
}
