using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_DetectCollision_PlaySound : H_DetectCollision
{
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
    }
}
