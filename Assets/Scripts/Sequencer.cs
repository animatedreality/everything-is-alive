using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Sequencer : MonoBehaviour
{
    protected enum SequencerType {
        Drum,
        Melody,
        Pad,
        None
    }

    protected SequencerType sequencerType;
    [Header("Audio")]
    public AudioClip clip;
    [Header("UI")]
    public Transform pointerSurface;
    public RectTransform canvasRect;
    public Slider playHeadSlider;
    public RectTransform playHeadHandle, playHeadArea;

    [HideInInspector]
    public GameObject notePrefab;

    [Header("Gameplay")]
    public Sequence firstSequence;
    public List<Sequence> sequences = new List<Sequence>();


    //default sequence length
    public int sequenceLength = 16;
    public int sequenceAmount = 1;

    [Header("Creature")]
    public CreatureData creatureData;
    public CreatureFamily creatureFamily;

    public virtual void Initialize(CreatureFamily _creatureFamily){

        creatureFamily = _creatureFamily;
        creatureData = creatureFamily.creatureData;

        //change the speed and length of each sequence based on SequencerType
        if(creatureData.creatureType == CreatureData.CreatureType.Drum){
            sequenceLength = 16;
        }else if(creatureData.creatureType == CreatureData.CreatureType.Pad){
            sequenceLength = 4;
        }

        //MAKE SURE TO ADDRESS THIS
        //this should be in inherented classes such as SequencerDrum
        // if(creatureData.creatureType == CreatureType.Drum){

        // }else if(creatureData.creatureType == CreatureType.Melody){
        //     sequencerType = SequencerType.Melody;
        //     sequenceAmount = 1;
        // }else if(creatureData.creatureType == CreatureType.Pad){
        //     sequencerType = SequencerType.Pad;
        //     sequenceAmount = creatureData.creatureMemberCount;
        // }else{
        //     sequencerType = SequencerType.None;
        //     sequenceAmount = 0;
        // }

        //if sequencerType is None, hide first sequence
        // if(sequencerType == SequencerType.None){
        //     firstSequence.gameObject.SetActive(false);
        //     return;
        // }

        if(firstSequence == null){
            Debug.LogError("firstSequence is null");
            return;
        }
        sequences.Add(firstSequence);
        //add all sequences
        PopulateSequences();
        //resize canvas after adding sequences
        ResizeCanvas();
        //add all notes
        InitializeSequences();
        //adjust slider
        InitializeSlider();
        //add self to AudioManager
        if(!AudioManager.i.sequencers.Contains(this)){
            AudioManager.i.sequencers.Add(this);
        }
    }

    void PopulateSequences(){
        sequenceAmount = creatureData.audioClips.Count;
        Debug.Log("PopulateSequences Set Amount: " + sequenceAmount);
        if(sequenceAmount > 1){
            //duplicate firstSequence based on sequenceAmount
            for(int i = 1; i < sequenceAmount; i++){
                GameObject newSequence = Instantiate(firstSequence.gameObject, firstSequence.transform.parent);
                newSequence.transform.localPosition = new Vector3(0, newSequence.transform.localPosition.y, 0 );
                newSequence.transform.localRotation = Quaternion.identity;
                newSequence.transform.localScale = new Vector3(1, 1, 1);
                Debug.Log("PopulateSequences: " + newSequence.name);
                sequences.Add(newSequence.GetComponent<Sequence>());
            }
        }

    }

    void ResizeCanvas(){
        canvasRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x, sequenceAmount * AudioManager.i.sequencerUIHeight);
        playHeadSlider.transform.localScale = new Vector3(
            playHeadSlider.transform.localScale.x,
            sequenceAmount * 2f - 1f,
            playHeadSlider.transform.localScale.z
        );
        pointerSurface.localScale = new Vector3(
            canvasRect.rect.width * canvasRect.localScale.x,
            canvasRect.rect.height * canvasRect.localScale.y,
            0.01f
        );
    }

    protected virtual void InitializeSequences(){
        //populate sequences with notes based on sequenceLength
        if(sequencerType == SequencerType.Drum || sequencerType == SequencerType.Pad){
            if(sequences.Count == creatureData.audioClips.Count){
                //assign audio clips to sequences
                for(int i = 0; i < sequences.Count; i++){
                    sequences[i].Initialize(this, sequenceLength, notePrefab, creatureData.audioClips[i]);
                }
            }else{
                Debug.LogError(creatureFamily.name + "Sequence count " + sequences.Count + " does not match audio clips count " + creatureData.audioClips.Count);
            }
        }
        //Need to initialize for other CreatureType and SequencerTypes
    }

    void InitializeSlider(){
        playHeadSlider.maxValue = sequenceLength - 1;
        playHeadSlider.value = 0;
        float newWidth = canvasRect.rect.width / sequenceLength;
        playHeadHandle.sizeDelta = new Vector2(newWidth, playHeadHandle.sizeDelta.y);
        //set the playhead area left and right padding to be half
        float padding = newWidth / 2;
        playHeadArea.offsetMin = new Vector2(padding, playHeadArea.offsetMin.y);
        playHeadArea.offsetMax = new Vector2(-padding, playHeadArea.offsetMax.y);
        Debug.Log("AdjustPlayHead: " + playHeadSlider.maxValue);
    }

    public void OnEveryStep(int _beatIndex){
        int localBeatIndex = ((int)_beatIndex / creatureData.sequenceLengthMultiplier) % sequenceLength;
        playHeadSlider.value = localBeatIndex;
    }

}
