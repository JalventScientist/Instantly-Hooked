using UnityEngine;

public class PersistentTutorialCreator : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    private void Awake()
    {
        if(GameObject.Find("TutCheck(Clone)") == null)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Destroy(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
}
