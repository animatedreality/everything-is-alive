using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateScalewithSamples_NS : MonoBehaviour
{
    public Vector3 originalScale;
    public float targetScaleGrowth = 0.2f;
    private Vector3 targetScale;
    public Transform objectToScale;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        //set targetScale to be originalScale multiplied by 1 + targetScaleGrowth
        targetScale = originalScale * (1f + targetScaleGrowth);
        if(!objectToScale)
        {
            objectToScale = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetAnimation(float _t){
        
        objectToScale.localScale = Vector3.Lerp(originalScale, targetScale, _t);
    }
}
