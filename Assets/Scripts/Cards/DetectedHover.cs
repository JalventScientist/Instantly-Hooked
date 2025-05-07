using UnityEngine;

public class DetectedHover : MonoBehaviour
{
    public bool Hovering = false;

    private void OnMouseOver()
    {
        Hovering = true;
    }
    private void OnMouseExit()
    {
        Hovering = false;
    }
}
