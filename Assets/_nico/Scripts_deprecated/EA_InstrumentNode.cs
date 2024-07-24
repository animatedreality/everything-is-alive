using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_InstrumentNode : MonoBehaviour
{
    public AudioSource audio;
    public EA_Sequencer sequencer;

    public virtual void OnInitialized(EA_Sequencer _sequencer){
        sequencer = _sequencer;
        audio = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void PlayNode(){
        audio.Play();
    }
}
