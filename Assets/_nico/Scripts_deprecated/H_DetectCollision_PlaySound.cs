using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class H_DetectCollision_PlaySound : H_DetectCollision
{
    public AudioSource audioSource;
    public bool scaleUpOnCollision = true;
    public float scaleAmount = 1.2f;
    public float duration = 0.2f;
    Vector3 originalScale, targetScale;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;
        targetScale = new Vector3(originalScale.x * scaleAmount, originalScale.y * scaleAmount, originalScale.z * scaleAmount); 

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void CollisionEvent(){
        if(audioSource != null){
            //check if audioSource is playing
            if(!audioSource.isPlaying){
                Debug.Log("Play Sound");
                audioSource.Play();
                //trigger visuals if there is
                if(GetComponent<H_DetectCollision_AnimateVisual>() != null){
                    GetComponent<H_DetectCollision_AnimateVisual>().AnimateVisual();
                }
            }
        }

        if(scaleUpOnCollision){
            transform.DOScale(targetScale, duration).OnComplete(() =>
            {   
                transform.DOScale(originalScale, duration);
            });
        }
    }
}
