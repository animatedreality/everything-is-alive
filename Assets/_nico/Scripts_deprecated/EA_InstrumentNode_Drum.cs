using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EA_InstrumentNode_Drum : EA_InstrumentNode
{
    
    // Start is called before the first frame update
    void Start()
    {
        
        base.Start();
    }

    public override void OnInitialized(EA_Sequencer _sequencer){
        base.OnInitialized(_sequencer);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void PlayNode(){
        base.PlayNode();

        //add to the sequencer
        sequencer.SetBeatSounds();
    }

}
