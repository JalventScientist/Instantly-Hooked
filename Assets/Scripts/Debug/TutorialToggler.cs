using TMPro;
using UnityEngine;

public class TutorialToggler : MonoBehaviour
{
    [SerializeField] TMP_Text statusChecker;

    TutorialLogger tutorialLogger;

    public void ToggleTutorial()
    {
        tutorialLogger.IncludeTutorial = !tutorialLogger.IncludeTutorial;
        statusChecker.text = tutorialLogger.IncludeTutorial ? "Tutorial Enabled (Click to toggle)" : "Tutorial Disabled (Click to toggle)";
    }

    void Start()
    {
        tutorialLogger = FindFirstObjectByType<TutorialLogger>();
        statusChecker.text = tutorialLogger.IncludeTutorial ? "Tutorial Enabled (Click to toggle)" : "Tutorial Disabled (Click to toggle)";
    }
}
