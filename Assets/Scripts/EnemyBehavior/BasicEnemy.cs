using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    ReadDeck readDeck;

    bool SpecialCardInDeck = false;

    private void Start()
    {
        readDeck = FindFirstObjectByType<ReadDeck>();
    }

    void CheckForSpecialCards()
    {

    }
}
