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
        Other
    }

    protected SequencerType sequencerType;
    [Header("Audio")]
    public float currentVolume = 1;
    float lastVolume = 0f;
    [Header("Volume Slider")]
    public Slider volumeSlider;
    public bool isMuted;
    [Header("UI")]
    public GameObject sequencerContainer;
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

    public void Initialize(CreatureFamily _creatureFamily){

        creatureFamily = _creatureFamily;
        creatureData = creatureFamily.creatureData;
        sequenceAmount = _creatureFamily.creatureData.creatureMemberCount;
        sequenceLength = _creatureFamily.creatureData.sequenceLength;

        //change the speed and length of each sequence based on SequencerType
        if(creatureData.creatureType == CreatureData.CreatureType.Drum){
            //sequenceLength = 16;
            sequencerType = SequencerType.Drum;
            notePrefab = AudioManager.i.drumNotePrefab;
        }else if(creatureData.creatureType == CreatureData.CreatureType.Pad){
            //sequenceLength = 4;
            sequencerType = SequencerType.Pad;
            notePrefab = AudioManager.i.padNotePrefab;
        }else if(creatureData.creatureType == CreatureData.CreatureType.Melody){
            //sequenceLength = 16;
            sequencerType = SequencerType.Melody;
            notePrefab = AudioManager.i.melodyNotePrefab;
        }else if(creatureData.creatureType == CreatureData.CreatureType.Stars){
            //IF THIS IS THE CASE, AUDIO WOULD NOT BE CONTROLLED BY AUDIOMANAGER!!!
            sequencerType = SequencerType.Other;
            sequencerContainer.SetActive(false);
            return;
        }

        //Everything related to actually using sequencer UI (special instrument does not need this)

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

    void InitializeSequences(){
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

    public void ToggleMute(){
        isMuted = !isMuted;
        if(isMuted){
            SetVolume(0);
        }else{
            SetVolume(currentVolume);
        }
    }

    public void SetVolume(float _volume){
        isMuted = (_volume == 0);
        if(!isMuted){
            if(creatureData.creatureType == CreatureData.CreatureType.Stars){
                SetVolumeInStars(_volume);
            }else{
                SetVolumeInSequences(_volume);
            }
            currentVolume = _volume;
            volumeSlider.value = currentVolume;
        }else{
            if(creatureData.creatureType == CreatureData.CreatureType.Stars){
                SetVolumeInStars(0);
            }else{
                SetVolumeInSequences(0);
            }
            volumeSlider.value = 0;
        }
    }

    private void SetVolumeInStars(float _volume){
        if(creatureData.creatureType == CreatureData.CreatureType.Stars){
            foreach(CreatureMemberStar member in creatureFamily.creatureMembers){
                member.audioSource.volume = _volume;
            }
        }
    }

    private void SetVolumeInSequences(float _volume){
        foreach(Sequence sequence in sequences){
            sequence.SetVolume(_volume);
        }
    }

}
