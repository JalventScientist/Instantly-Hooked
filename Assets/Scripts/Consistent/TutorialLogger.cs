using UnityEngine;

public class TutorialLogger : MonoBehaviour
{
    public bool IncludeTutorial = true; // Set to false to immediately start with face cards enabled
    public bool isFirstEverTurn = true; // Set to false to skip the first turn tutorial

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
