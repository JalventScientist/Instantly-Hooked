using NUnit.Framework;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class VisualizeHands : MonoBehaviour
{
    private Hands hands;

    private GameObject cardBase;
    private RectTransform cardRect;

    private List<GameObject> visualPlayerHand = new List<GameObject>();
    private List<GameObject> visualEnemyHand = new List<GameObject>();

    private Transform playerHandCenter;
    private Transform enemyHandCenter;
    [SerializeField] private float cardSpacing;
    private void Start()
    {
        playerHandCenter = GameObject.FindAnyObjectByType<Tag_PlayerHand>().transform;
        enemyHandCenter = GameObject.FindAnyObjectByType<Tag_EnemyHand>().transform;
        cardBase = Resources.Load<GameObject>("Prefabs/Cards/Card");
        cardRect = cardBase.GetComponent<RectTransform>();
        if (!TryGetComponent<Hands>(out hands))
        {
            hands = FindAnyObjectByType<Hands>();
        }
        else
        {
            hands = GetComponent<Hands>();
        }
    }
    private void Update()
    {
        /*bool isNewPlayerHand = visualPlayerHand.Count != hands.playerHand.Count;
        if (isNewPlayerHand)
        {
            for (int i = 0; i < visualPlayerHand.Count; i++)
            {
                Instantiate(cardBase, playerHandCenter.position + new Vector3(cardRect.rect.width / 2 + (visualPlayerHand.Count / 2 - visualPlayerHand.Count) / (1 / cardSpacing), 0, 0), new Quaternion(0, 0, 0, 0));
            }
        }*/
    }
}
