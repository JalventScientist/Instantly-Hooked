using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    private Decks decks;
    [SerializeField] private int startingHand;

    public List<string> playerHand = new List<string>();
    public List<string> enemyHand = new List<string>();

    private void Start()
    {
        decks = GetComponent<Decks>();
        for (int i = 0; i < startingHand; i++)
        {
            Util.DrawFromDeckIntoHand(decks.drawDeck, playerHand);
            Util.DrawFromDeckIntoHand(decks.drawDeck, enemyHand);
        }
    }
}
