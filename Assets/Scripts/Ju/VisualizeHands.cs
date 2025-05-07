using NUnit.Framework;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeHands : MonoBehaviour
{
    private Hands hands;

    private GameObject cardBase;

    private List<GameObject> visualPlayerHand = new List<GameObject>();
    private List<GameObject> visualEnemyHand = new List<GameObject>();
    private void Start()
    {
        cardBase = Resources.Load<GameObject>("Prefabs\\Cards\\Card.prefab");
        hands = FindAnyObjectByType<Hands>();
    }
    private void Update()
    {
        bool isNewPlayerHand = visualPlayerHand.Count != hands.playerHand.Count;
        if (!isNewPlayerHand)
        {
            for (int i = 0; i < visualPlayerHand.Count; i++)
            {

            }
        }
    }
}
