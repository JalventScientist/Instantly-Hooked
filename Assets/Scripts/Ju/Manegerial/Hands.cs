using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Util.DrawFromDeckIntoHand(decks.drawDeck, playerHand);
        if (Input.GetKeyDown(KeyCode.E))
            Util.DrawFromDeckIntoHand(decks.drawDeck, enemyHand);
        if (Input.GetKeyDown(KeyCode.Z))
            Util.DrawFromDeckIntoHand(playerHand, decks.discardDeck);
        if (Input.GetKeyDown(KeyCode.C))
            Util.DrawFromDeckIntoHand(enemyHand, decks.discardDeck);
        if (Input.GetKeyDown(KeyCode.X))
            Util.ShuffleFromDeckIntoDeck(decks.drawAndDiscardDeck, decks.drawDeck);
        if (Input.GetKeyDown(KeyCode.V))
            Util.ShuffleFromDeckIntoDeck(decks.drawDeck, decks.drawDeck);
    }
}
