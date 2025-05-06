using System.Collections.Generic;
using UnityEngine;

public class ReadDeck : MonoBehaviour
{
    [SerializeField] Decks cardDeck;

    List<Card> PlayerDeck = new List<Card>();
    List<Card> DiscardDeck = new List<Card>();

    private void Start()
    {
        cardDeck = FindFirstObjectByType<Decks>();
    }

    public void PullDeck(bool playerCard)
    {

    }
}
