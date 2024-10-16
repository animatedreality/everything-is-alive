using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AudioManager : MonoBehaviour
{
    public static AudioManager i;
    public float bpm = 120f;

    private double time;

    private double nextEventTime;

    public bool globalPlay = false;

    public UnityEvent<int> OnEveryStep;
    public List<Sequence> sequences;
    public List<Sequencer> sequencers;

    [HideInInspector]
    public int beatIndex = 0;

    // public delegate void OnEveryStepDelegate(int x);

    // public event OnEveryStepDelegate OnEveryStepEvent;

    private void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float sequencerUIHeight = 108.33f;
    [Header("Prefabs")]
    public GameObject drumNotePrefab;
    public GameObject melodyNotePrefab;
    public GameObject padNotePrefab;

    [Header("Audio")]
    public List<AudioClip> audioClips = new List<AudioClip>();
    public void Initialize()
    {
        nextEventTime = AudioSettings.dspTime + 0.4;
    }

    private void Reset()
    {
        if (OnEveryStep == null)
        {
            OnEveryStep = new UnityEvent<int>();
        }
    }

    public void Update()
    {
        time = AudioSettings.dspTime;
        double sixteenthNote = 60.0 / (bpm * 4);
        if (globalPlay)
        {
            if (time + sixteenthNote > nextEventTime)
            {
                foreach(Sequence sequence in sequences)
                {
                    int localBeatIndex = beatIndex % sequence.sequenceLengthMultiplier;

                    // Only schedule for sequences that should trigger on this beat
                    if (localBeatIndex == 0)
                    {
                        Debug.Log(sequence.sequencer.creatureFamily.name + " Scheduling sequence Play " + beatIndex);
                        sequence.Schedule(beatIndex, nextEventTime);
                    }
                }
                
                nextEventTime += sixteenthNote;
                beatIndex++;

                TimeEvent te = new TimeEvent(nextEventTime, EveryStepAction, beatIndex);
                StartCoroutine(CheckEventRoutine(te));
            }
        }
    }

    private int lateUpdateCount = 0;

    private void LateUpdate()
    {
        if (!globalPlay) //occasionally update stuff when globalPlay is false
        {
            if (lateUpdateCount % 64 == 0)
            {
                EveryStepAction(AudioSettings.dspTime, beatIndex);
            }
            lateUpdateCount++;
        }
    }

    private IEnumerator CheckEventRoutine(TimeEvent timeEvent)
    {
        yield return new WaitUntil(() => AudioSettings.dspTime >= timeEvent.time);
        timeEvent.callback(timeEvent.time, timeEvent.beatIndex);
    }

    public void EveryStepAction(double time, int x)
    {
        if (time < AudioSettings.dspTime - 0.1) //if the time is too far in the past it's too late to run the action
        {
            return;
        }
        OnEveryStep?.Invoke(x);
        // OnEveryStepEvent?.Invoke(x);
        foreach(Sequencer sequencer in sequencers){
            sequencer.OnEveryStep(x);
        }
    }

    public void Play()
    {
        beatIndex = -1;

        nextEventTime = AudioSettings.dspTime;
        globalPlay = true;
    }

    public void Stop()
    {
        globalPlay = false;
    }

}
