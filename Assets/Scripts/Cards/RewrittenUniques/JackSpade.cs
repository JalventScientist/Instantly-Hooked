using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JackSpade : Card
{
    ReadDeck deck;
    TMP_Text tooltip;

    string tooltiptxt = "Play 2 cards";

    float ErrorTime = 0f;

    private void Start()
    {
        deck = FindFirstObjectByType<ReadDeck>();
        tooltip = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
    }

    public override void ApplyCard()
    {

        if(CanbePlayed() == false)
        {
            ErrorTime += 1f;
            return;
        } else
        {
            base.ApplyCard();
        }

    }

    bool CanbePlayed()
    {
        bool EndValue;
        List<string> NormalCards = new List<string>();
        if (IsPlayerCard)
        {
            foreach(GameObject card in deck.PlayerDeck)
            {
                Card component = card.GetComponent<Card>();
                if (component.isSpecial == false && component.UsedCard == false)
                    NormalCards.Add(card.name);
            }
            if(NormalCards.Count > 1)
            {
                EndValue = true;
            } else
            {
                EndValue = false;
            }
        } else
        {
            EndValue = true;
        }

        return EndValue;
    }

    public override void OverrideUpdate()
    {
        if(ErrorTime > 0f)
        {
            tooltip.text = "Not enough cards!";
            ErrorTime -= Time.deltaTime;
        } else
        {
            tooltip.text = tooltiptxt;
        }
    }

}
