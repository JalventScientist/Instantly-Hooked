using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    [SerializeField] string TargetScene;


    public void LoadScene()
    {
        SceneManager.LoadScene(TargetScene);
    }
}
