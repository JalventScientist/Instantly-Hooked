using UnityEngine;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;

public class ThrowCard : MonoBehaviour
{
    public void RenderProperCard(string CardName, bool isPlayerCard)
    {
        Sprite ConfirmSprite = Resources.Load<Sprite>("CardImages/" + CardName);
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("CardImages/" + CardName);
        StartCoroutine(animateThrow(isPlayerCard));

    }
    IEnumerator animateThrow(bool isPlayerCard)
    {
        transform.rotation = Quaternion.Euler(90, 0, Random.Range(0f,360f));
        transform.DOLocalMoveZ(isPlayerCard ? -2 : 2, 0.3f).SetEase(Ease.OutQuad);
        transform.DOLocalRotate(new Vector3(90, 0, Random.Range(0f, 360f)), 0.3f).SetEase(Ease.OutQuad);
        yield return null;
    }

    public void DestroyCard()
    {
        Transform Target = GameObject.Find("DiscardDeck").transform;
        transform.DOLocalMove(new Vector3(Target.position.x, transform.position.y, Target.position.z), 0.3f).SetEase(Ease.OutQuad).OnComplete(()=>Destroy(gameObject));
    }
}
