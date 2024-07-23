using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FO_InstrumentObject : MonoBehaviour
{
    //Controls how instrument plays sounds
    //Control how instrument make visual changes (face emoji, colors, etc...)
    public PlayFaceAnimation faceAnimation;
    bool isPlaying = false;
    void Start()
    {
        
    }

    public void TriggerSound(){
        if(GetComponent<AudioSource>()){
            //check if AudioSource is playing
            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().Play();
                Debug.Log("Play Audio " + name);
                isPlaying = true;
            }
        }
        if(faceAnimation){
            faceAnimation.PlayAnimation("sing");
        }
    }

    //detect if audiosource has stopped, and trigger an animation on audio stop
    void Update(){
        if(GetComponent<AudioSource>()){
            //check if AudioSource is playing
            if (!GetComponent<AudioSource>().isPlaying && isPlaying)
            {
                Debug.Log("Audio stopped");
                if(faceAnimation){
                    faceAnimation.PlayAnimation("idle");
                }
                isPlaying = false;
            }
        }
    }
}
