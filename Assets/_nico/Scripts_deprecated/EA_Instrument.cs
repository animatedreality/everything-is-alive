using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EA_Instrument : MonoBehaviour
{
    //attached to each instrument object

    // public string instrumentName;
    // public AudioClip[] instrumentSounds;
    // public AudioSource[] instrumentAudioSources;
    // public bool isActive;
    // public bool isBeingPlayed;

    //PLAY RULES
    //play rules TBD

    //sequencer vertical heights
    public List<EA_Sequencer> sequencers = new List<EA_Sequencer>();
    public GameObject sequencerContainer;
    public EA_InstrumentNode[] instrumentNodes;
    public float sequencerWidth;
    public float sequencerHeight;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        OnInitialized();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public void OnInitialized(){
        //sequencer.OnInitialized(this);
        int barsPerLoop = 4;
        int beatsPerBar = 4;
        sequencerWidth = EA_SequenceManager.s.sequencerBeatWidthSpacing * barsPerLoop * beatsPerBar;
        sequencerHeight = EA_SequenceManager.s.sequencerVerticalSpacing * instrumentNodes.Length;

        for (int i = 0; i < instrumentNodes.Length; i++){

            GameObject sequenceObject = Instantiate(EA_SequenceManager.s.sequencerPrefab, sequencerContainer.transform);
            sequenceObject.name = "Sequencer" + instrumentNodes[i].name;
            
            sequenceObject.transform.localPosition = new Vector3(0, i * EA_SequenceManager.s.sequencerVerticalSpacing, -sequencerWidth / 2);
            sequenceObject.GetComponent<EA_Sequencer>().OnInitialized(this,barsPerLoop, beatsPerBar, instrumentNodes[i], i);
            sequencers.Add(sequenceObject.GetComponent<EA_Sequencer>());
            EA_SequenceManager.s.sequencers.Add(sequenceObject.GetComponent<EA_Sequencer>());

            //initialize instrumentNodes with the corresponding sequencer
            instrumentNodes[i].OnInitialized(sequenceObject.GetComponent<EA_Sequencer>());
        }
    }
}
