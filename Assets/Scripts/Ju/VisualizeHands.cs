using NUnit.Framework;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;

public class VisualizeHands : MonoBehaviour
{
    private Hands hands;

    private GameObject cardBase;
    private RectTransform cardRect;

    [SerializeField] private List<GameObject> visualPlayerHand = new List<GameObject>();
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
        RenderHand(visualPlayerHand, hands.playerHand, playerHandCenter, true);
        RenderHand(visualEnemyHand, hands.enemyHand, enemyHandCenter);
    }

    private void RenderHand(List<GameObject> visualHandList, List<string> textHandList, Transform center, bool isPlayerHand = false)
    {
        bool isNewHand = visualHandList.Count != textHandList.Count;
        if (isNewHand)
        {
            foreach (GameObject card in visualHandList)
                Destroy(card);
            visualHandList.Clear();
            for (int i = 0; i < textHandList.Count; i++)
            {
                GameObject card = Instantiate(cardBase, center.position + new Vector3(cardRect.rect.width / 2 + ((i - textHandList.Count / 2) / (1 / cardSpacing)), 0, 0), new Quaternion(0, 0, 0, 0), center);
                card.name = textHandList[i];
                Card cardCard = card.GetComponent<Card>();
                if (isPlayerHand)
                {
                    cardCard.IsPlayerCard = true;
                }
                cardCard.SetupCard();
                visualHandList.Add(card);
            }
        }
    }
}
