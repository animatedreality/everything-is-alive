using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Sequencer : MonoBehaviour
{
    public int barsPerLoop = 4;//total amount of bars for this sequencer
    public int beatsPerBar = 4;
    public float beatWidthSpacing;
    public float barLength;
    List<EA_Bar> bars = new List<EA_Bar>();
    List<Transform> sequenceLines = new List<Transform>();
    public EA_Instrument instrument;

    //beats
    public int currentBeat = 0;
    private int lastBeat = -1;
    public int currentBar = 0;
    private int lastBar = 0;

    //playback
    public float playbackTime;
    public float totalLoopTime;
    public float globalPlaybackTime;
    public GameObject playbackIndicator;
    Vector3 startPos, endPos;

    bool isInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateSequencer(){
        if(!isInitialized) return;
        globalPlaybackTime = EA_SequenceManager.s.playbackTime;
        playbackTime = globalPlaybackTime % totalLoopTime;
        SetPlaybackIndicator();
        //total beat amount is barsPerLoop * beatsPerBar
        //currentBeat is the current beat index
        //set currentBeat as playbackTime / totalLoopTime
        currentBeat = (int)(playbackTime / totalLoopTime * barsPerLoop * beatsPerBar);
        currentBeat %= (barsPerLoop * beatsPerBar);

        if(currentBeat != lastBeat){
            OnBeat();
            lastBeat = currentBeat;
        }
    }

    public void OnInitialized(EA_Instrument _instrument, int _barsPerLoop, int _beatsPerBar, EA_InstrumentNode _NodeObject, int _NodeIndex){
        instrument = _instrument;
        barsPerLoop = _barsPerLoop;
        beatsPerBar = _beatsPerBar;
        beatWidthSpacing = EA_SequenceManager.s.sequencerBeatWidthSpacing;
        barLength = beatWidthSpacing * beatsPerBar;

        //generate bars
        for(int i = 0; i < barsPerLoop; i++){
            GameObject bar = Instantiate(EA_SequenceManager.s.barPrefab, transform);
            EA_Bar barScript = bar.GetComponent<EA_Bar>();
            barScript.OnInitialized(this, beatsPerBar, beatWidthSpacing, i, _NodeObject, _NodeIndex);
            bar.transform.localPosition = new Vector3(0,0,barLength * i);
            bars.Add(barScript);
            //when it's the last bar, get the barEnd's transform position from it
            if(i == barsPerLoop - 1){
                endPos = barScript.barEnd.transform.position;
            }
        }
        startPos = transform.position;
        totalLoopTime = barsPerLoop * beatsPerBar * EA_SequenceManager.s.beatInterval;
        isInitialized = true;

    }

    void SetPlaybackIndicator(){
        playbackIndicator.transform.position = Vector3.Lerp(startPos, endPos, playbackTime / totalLoopTime);
    }

    //this is called when the current beat is set
    public void SetBeatSounds(){
        //when this is called, call the SetSound funtion in current beat
        bars[currentBar].beats[currentBeat % beatsPerBar].SetSound(true);
    }

    public void OnBeat(){
        currentBar = currentBeat / beatsPerBar;
        //currentBeat ++;
        //Debug.Log("Current beat: " + currentBeat);
        //play all the beats in EA_Bar
        bars[currentBar].OnBeat(currentBeat % beatsPerBar);
        if(lastBar != currentBar){
            bars[lastBar].OnReset();
            lastBar = currentBar;
        }
    }

    public void OnReset(){
        foreach(EA_Bar bar in bars){
            bar.OnReset();
        }
    }
}

