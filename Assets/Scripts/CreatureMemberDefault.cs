using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMemberDefault : CreatureMember
{

    //DEFAULT CREATURE == DRUM & PAD SEQUENCER
    public H_DetectCollision h_detectCollision;
    [Header("Customize")]
    public bool scaleUpOnPlay = true;
    public void Initialize(Sequence _sequence)
    {
        sequence = _sequence;
        //assign clip from creatureData
        clip = _sequence.clip;
        //append the OnPlay function to the OnPlay event in Sequence
        sequence.OnPlay.AddListener((int value) => OnPlay());

        //add OnPlay to touch
        if(!h_detectCollision){
            h_detectCollision = GetComponent<H_DetectCollision>();
        }
        if(!h_detectCollision){
            h_detectCollision = GetComponentInChildren<H_DetectCollision>();
        }
        if(h_detectCollision){
            h_detectCollision.collisionEnterEvent.AddListener(() => OnPlay());
            h_detectCollision.collisionEnterEvent.AddListener(() => sequence.PlayAudio());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlay(){
        Debug.Log("OnPlay: " + name);
        if(scaleUpOnPlay){
            if(GetComponent<ScaleOnPlay>()){
                GetComponent<ScaleOnPlay>().ScaleUp();
            }
            if(GetComponentInChildren<ScaleOnPlay>()){
                GetComponentInChildren<ScaleOnPlay>().ScaleUp();
        }
        }
        base.OnPlay();
    }
}
