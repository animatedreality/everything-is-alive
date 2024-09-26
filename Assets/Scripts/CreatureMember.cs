using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMember : MonoBehaviour
{
    public CreatureData creatureData;
    public Sequencer sequencer;
    public Sequence sequence;//NOT ALL CREATURE MEMBERS NEED SEQUENCE
    public virtual void Initialize(Sequence _sequence){

    }
    public virtual void OnPlay(){

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
