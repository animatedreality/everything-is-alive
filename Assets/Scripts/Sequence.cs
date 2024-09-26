using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sequence : MonoBehaviour
{
    public int sequenceLength;
    public GameObject notePrefab;
    public List<int> sequencePattern;//edit this from Note script
    public List<Note> notes;
    public UnityEvent<int> OnPlay;
    public List<AudioSource> sources = new List<AudioSource>();

    //from Sequencer
    public Sequencer sequencer;
    public AudioClip clip;
    public void Initialize(Sequencer _sequencer, int _sequenceLength, GameObject _notePrefab, AudioClip _clip){
        //initialize sequenceLength
        sequenceLength = _sequenceLength;
        notePrefab = _notePrefab;
        clip = _clip;
        sequencer = _sequencer;
        //populate notes
        for(int i = 0; i < sequenceLength; i++){
            GameObject newNote = Instantiate(notePrefab, transform);
            newNote.GetComponent<Note>().Initialize(this);
        }
        if(!AudioManager.i.sequences.Contains(this)){
            AudioManager.i.sequences.Add(this);
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
        for (int i = 0; i < sequencePattern.Count; i++)
        {
            int localBeatIndex = _beatIndex % sequenceLength;
            if (sequencePattern[i] == localBeatIndex)
            {
                PlayAtTime(nextEventTime, localBeatIndex);
            }
        }
    }

    public void PlayAtTime(double time, int localBeatIndex)
    {
        if (time <= AudioSettings.dspTime)
            return;
        AudioSource source = GetAvailableSource();
        source.PlayScheduled(time);
        StartCoroutine(OnPlayCoroutine(time, localBeatIndex));
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
