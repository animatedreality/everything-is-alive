using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sequence : MonoBehaviour
{
    public int sequenceLength;
    public GameObject notePrefab;
    public List<int> sequencePattern;//edit this from Note script, contains indexes of notes that are on
    public List<Note> notes;
    public UnityEvent<int> OnPlay;
    public List<AudioSource> sources = new List<AudioSource>();

    //from Sequencer
    public Sequencer sequencer;
    public AudioClip clip;
    public int sequenceLengthMultiplier;
    public void Initialize(Sequencer _sequencer, int _sequenceLength, GameObject _notePrefab, AudioClip _clip){
        //initialize sequenceLength
        //Debug.Log("Initializing Sequence");
        sequenceLength = _sequenceLength;
        notePrefab = _notePrefab;
        clip = _clip;
        sequencer = _sequencer;
        sequenceLengthMultiplier = sequencer.creatureData.sequenceLengthMultiplier;
        //populate notes
        for(int i = 0; i < sequenceLength; i++){
            GameObject newNote = Instantiate(notePrefab, transform);
            newNote.GetComponent<Note>().Initialize(this);
        }
        if(!AudioManager.i.sequences.Contains(this)){
            AudioManager.i.sequences.Add(this);
           // Debug.Log("Adding to AudioManager");
        }

        //TO BE REMOVED, TESTING ONLY!!!!!!!!!!!!!!!!
        for(int i=0; i<notes.Count; i++){
            if(i == Random.Range(0, notes.Count)){
                notes[i].SetState(true);
            }
        }
    }

    public void Schedule(int _beatIndex, double nextEventTime)
    {
        //Debug.Log(sequencer.creatureFamily.name + " Scheduling sequence Play " + _beatIndex);
        int localBeatIndex = ((int)_beatIndex / sequenceLengthMultiplier) % sequenceLength;
        //Debug.Log(sequencer.creatureFamily.name + " Scheduling sequence Play " + localBeatIndex);
        // Only play if the beat index aligns with the sequenceLengthMultiplier interval.
        if (sequencePattern.Contains(localBeatIndex))
        {
            PlayAtTime(nextEventTime, localBeatIndex);
        }
    }


    public void PlayAtTime(double time, int localBeatIndex)
    {
        if (time <= AudioSettings.dspTime)
            return;

        Debug.Log(sequencer.creatureData.name + "Playing at time " + time + " with beat index " + localBeatIndex);
        AudioSource source = GetAvailableSource();
        source.PlayScheduled(time);
        StartCoroutine(OnPlayCoroutine(time, localBeatIndex));
    }

    public void PlayAudio(){
        AudioSource source = GetAvailableSource();
        source.Play();
    }

    IEnumerator OnPlayCoroutine(double time, int localBeatIndex)
    {
        yield return new WaitForSeconds((float)(time - AudioSettings.dspTime));
        OnPlay?.Invoke(localBeatIndex);
    }

    public void AddToSequencePattern(int index){
        sequencePattern.Add(index);
    }

    public void RemoveFromSequencePattern(int index){
        sequencePattern.Remove(index);
    }

    void OnDestroy(){
        AudioManager.i.sequences.Remove(this);
    }

    void Reset(){
        if(OnPlay == null){
            OnPlay = new UnityEvent<int>();
        }
    }

    public void SetVolume(float _volume){
        foreach(AudioSource source in sources){
            source.volume = _volume;
        }
    }

    //------------AudioSources-------------
    AudioSource GetAvailableSource()
    {
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return CreateNewSource();
    }

    AudioSource CreateNewSource(){
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.playOnAwake = false;
        source.loop = false;
        sources.Add(source);
        return source;
    }

}
