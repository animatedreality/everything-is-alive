using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleOnPlay : MonoBehaviour
{
    public float originalScale = 1f;
    public float targetScale = 1.2f;
    public float duration = 0.2f;

    public void ScaleUp()
    {
        //briefly scale up the object
        transform.DOScale(targetScale, duration).OnComplete(() =>
        {
            //scale back down
            transform.DOScale(originalScale, duration);
        });
    }
}
