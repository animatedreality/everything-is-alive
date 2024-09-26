using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequencerDrum : Sequencer
{

    //Initialize is called from Creature
    public override void Initialize(CreatureFamily _creatureFamily)
    {
        sequencerType = SequencerType.Drum;
        sequenceAmount = _creatureFamily.creatureData.creatureMemberCount;
        notePrefab = AudioManager.i.drumNotePrefab;
        base.Initialize(_creatureFamily);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // protected override void SetupSequences(){
    //     //populate sequences with notes based on sequenceLength
    //     for(int i = 0; i < sequenceLength; i++){

    //     }
    // }

}
