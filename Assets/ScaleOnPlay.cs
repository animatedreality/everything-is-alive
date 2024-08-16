using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleOnPlay : MonoBehaviour
{

    public void ScaleUp()
    {
        //briefly scale up the object
        transform.DOScale(1.2f, 0.2f).OnComplete(() =>
        {
            //scale back down
            transform.DOScale(1f, 0.2f);
        });
    }
}
