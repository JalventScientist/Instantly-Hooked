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
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f,360f));
        transform.DOLocalMoveX(isPlayerCard ? -3 : 3, 0.3f).SetEase(Ease.OutQuad);
        transform.DOLocalRotate(new Vector3(0, 0, Random.Range(0f, 360f)), 0.3f).SetEase(Ease.OutQuad);
        yield return null;
    }

    public void DestroyCard()
    {

        transform.DOLocalMoveY(-10f, 0.3f).SetEase(Ease.InQuad).OnComplete(()=> Destroy(gameObject));
    }
}
