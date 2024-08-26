using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleOnPlay : MonoBehaviour
{
    public float scaleAmount = 1.2f;
    public float duration = 0.2f;
    Vector3 originalScale, targetScale;
    
    void Start(){
        originalScale = transform.localScale;
        targetScale = new Vector3(originalScale.x * scaleAmount, originalScale.y * scaleAmount, originalScale.z * scaleAmount); 
    }

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
