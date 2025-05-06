using Unity.VisualScripting;
using UnityEngine;

public class GameManagerBase : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
