using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Bar : MonoBehaviour
{   
    //TEMP DEBUGGING OBJ
    public EA_Sequencer sequencer;
    public GameObject barStart, barEnd;
    public List<EA_Beat> beats = new List<EA_Beat>();
    public Vector3 nextBarPosition;
    public int barIndex;

    //Node related
    public int instrumentNodeIndex;
    public EA_InstrumentNode instrumentNode;

    public int currentBeatIndex;
    // public float barLength;
    // public float beatLength;
    // public int beatsPerBar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInitialized(EA_Sequencer _sequencer, int _beatsPerBar, float _beatLength, int _barIndex, EA_InstrumentNode _instrumentNode, int _nodeIndex){
        barIndex = _barIndex;
        gameObject.name = "Bar" + _barIndex + "of" + _instrumentNode.gameObject.name;
        Debug.Log("OnInitialized" + name);
        //Initialize Beats
        sequencer = _sequencer;
        instrumentNodeIndex = _nodeIndex;
        instrumentNode = _instrumentNode;
        
        //generate beatsPerBar amount of beats by spawning the beatPrefab from SequenceManager and at the distance of (0,0,beatLength * index)
        for(int i = 0; i < _beatsPerBar; i++){
            Debug.Log("Instantiate beat " + i + " of " + _beatsPerBar);
            Debug.Log("beat parent is " + transform);
            GameObject beat = Instantiate(EA_SequenceManager.s.beatPrefab, transform);
            beat.transform.localPosition = new Vector3(0,0,_beatLength * i);
            beat.GetComponent<EA_Beat>().OnInitialized(i, this);
            beats.Add(beat.GetComponent<EA_Beat>());
        }
        // nextBarPosition = transform.position + new Vector3(0,0,barLength);
        // barEnd.transform.position = nextBarPosition;
        // beatsPerBar = _beatsPerBar;
        // beatLength = _beatLength;
        // barLength = _barLength;
    }

    public void OnBeat(int beatIndex){
        currentBeatIndex = beatIndex;
        for(int i = 0; i < beats.Count; i++){
            if(i != beatIndex){
                beats[i].PlayBeat(false);
            }else{
                beats[i].PlayBeat(true);
            }
        }
    }

    public void ClearLastBeat(){
        beats[beats.Count - 1].PlayBeat(false);
    }

    //all beats visuals reseted to off
    public void OnReset(){
        for(int i = 0; i < beats.Count; i++){
            beats[i].PlayBeat(false);
        }
    }
}
