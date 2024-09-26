using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMemberDefault : CreatureMember
{

    //DEFAULT CREATURE == DRUM & PAD SEQUENCER
    public AudioClip clip;
    // Start is called before the first frame update
    public override void Initialize(Sequence _sequence)
    {
        sequence = _sequence;
        //assign clip from creatureData
        clip = _sequence.clip;
        //append the OnPlay function to the OnPlay event in Sequence
        sequence.OnPlay.AddListener((int value) => OnPlay());
        base.Initialize(_sequence);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlay(){
        Debug.Log("OnPlay: " + name);
        if(GetComponentInChildren<ScaleOnPlay>()){
            GetComponentInChildren<ScaleOnPlay>().ScaleUp();
        }
        base.OnPlay();
    }
}
