using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_DetectCollision_AnimateVisual : MonoBehaviour
{
    //use curve to create a tween animation of the scale of this object so that it bounces to 1.2 and then return to 1
    public AnimationCurve scaleCurve;
    public float scaleMultiplier = 1.5f;
    public float scaleAnimationDuration = 1f;
    public float scaleAnimationDelay =0f;
    private Vector3 originalScale;

    bool startAnimating = false;
    float animationStartTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //use curve to create a tween animation of the scale of this object so that it bounces to 1.2 and then return to 1
        if(startAnimating){
            float scaleValue = scaleCurve.Evaluate(Time.time - animationStartTime - scaleAnimationDelay);
            transform.localScale = originalScale * (1 + scaleValue * scaleMultiplier);
        }
    }

    public void AnimateVisual(){
        Debug.Log("AnimateVisual");
        //set animationStartTime to now
        animationStartTime = Time.time;
        startAnimating = true;
        Invoke("ResetAnimation", scaleAnimationDuration);
    }

    void ResetAnimation(){
        startAnimating = false;
    }
}
