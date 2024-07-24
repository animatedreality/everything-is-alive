using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_SequenceManager : MonoBehaviour
{
    public static EA_SequenceManager s { get; private set; }
    // Start is called before the first frame update
    public GameObject beatPrefab;
    public GameObject barPrefab;
    public GameObject sequencerPrefab;
    public List<EA_Sequencer> sequencers = new List<EA_Sequencer>();

    public float playbackTime;//global variable for all sequencer to track play time in Seconds
    public bool isPlaying;
    public float bpm = 120f;
    public float beatInterval;
    //private float lastBeatTime;
    //public int beatIndex;

    //Sequencer view
    public float sequencerVerticalSpacing = 0.1f;
    public float sequencerBeatWidthSpacing = 0.1f;

    private void Awake(){
        if (s == null)
        {
            s = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (s != this)
        {
            Destroy(gameObject);
        }
        beatInterval = 60f / bpm; // Calculate the interval between beats based on bpm
        //lastBeatTime = -beatInterval; // Initialize lastBeatTime to ensure the first beat is detected immediately
    }

    void Start()
    {
        isPlaying = true;
    }

    void Update()
    {
        if (isPlaying)
        {
            beatInterval = 60f / bpm;//adopting to changing bpm
            playbackTime += Time.deltaTime;
            // // Check if this is the first update call after isPlaying has been set to true
            // if (lastBeatTime == 0 && playbackTime < beatInterval) {
            //     OnBeat(); // Call OnBeat immediately
            //     lastBeatTime = playbackTime; // Set lastBeatTime to current playbackTime
            // } else if (playbackTime - lastBeatTime >= beatInterval) {
            //     lastBeatTime += beatInterval;
            //     OnBeat();
            // }

            foreach(EA_Sequencer sequencer in sequencers){
                sequencer.UpdateSequencer();
            }
        }
        else
        {
            OnPause();
        }
    }

    public void OnPause(){
        playbackTime = 0;
        // beatIndex = 0;
        // lastBeatTime = 0;
        foreach(EA_Sequencer sequencer in sequencers){
            sequencer.OnReset();
        }
    }

    // public void OnBeat()
    // {
    //     // This function is called every beat
    //     // Add functionality here that should happen on every beat
        
    //     //Debug.Log("Beat hit at time: " + playbackTime);
    //     // beatIndex ++;

    //     //call OnBeat function in all sequencers
    //     foreach (EA_Sequencer sequencer in sequencers)
    //     {
    //         sequencer.OnBeat();
    //     }

       
    // }

}
