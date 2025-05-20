using System.Collections;
using UnityEngine;
using DG.Tweening;

public class AnimateCam : MonoBehaviour
{
    [SerializeField] Transform BasePos;
    [SerializeField] Transform EvalPos;


    public void ToPos(bool IsEval)
    {
        if (IsEval)
        {
            transform.DOMove(EvalPos.position, .5f).SetEase(Ease.InOutQuad);
            transform.DORotate(EvalPos.rotation.eulerAngles, .5f).SetEase(Ease.InOutQuad);
        }
        else
        {
            transform.DOMove(BasePos.position, .5f).SetEase(Ease.InOutQuad);
            transform.DORotate(BasePos.rotation.eulerAngles, .5f).SetEase(Ease.InOutQuad);
        }
    }
}
