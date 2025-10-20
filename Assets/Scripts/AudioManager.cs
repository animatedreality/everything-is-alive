using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
public class AudioManager : MonoBehaviour
{
    public static AudioManager i;

    [Header("Timing")]
    public float bpm = 120f;
    private double time;
    private double nextEventTime;
    public bool globalPlay = false;

    [Header("Audio Management")]
    public AudioSourcePool audioSourcePool;
    public SoundLibrary soundLibrary;
    public AudioMixerGroup masterMixerGroup;

    [Header("Performance")]
    [SerializeField] private int maxSimultaneousSounds = 10;
    private List<PlayingSoundInfo> playingSounds = new List<PlayingSoundInfo>();

    // Existing fields...
    public UnityEvent<int> OnEveryStep;
    public List<Sequence> sequences;
    public List<Sequencer> sequencers;
    [HideInInspector] public int beatIndex = 0;
    private AudioSource audioClipsMenuSource;

    public float sequencerUIHeight = 108.33f;
    [Header("Prefabs")]
    public GameObject drumNotePrefab;
    public GameObject melodyNotePrefab;
    public GameObject padNotePrefab;

    [Header("Audio")]
    public List<AudioClip> audioClips = new List<AudioClip>();

    private struct PlayingSoundInfo
    {
        public AudioSource source;
        public float startTime;
        public string soundName;
    }

    private void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSystem()
    {
        if (audioSourcePool == null)
        {
            GameObject poolObject = new GameObject("AudioSourcePool");
            poolObject.transform.SetParent(transform);
            audioSourcePool = poolObject.AddComponent<AudioSourcePool>();
        }
    }

    public AudioSource PlaySound(string soundName, Vector3 position = default)
    {
        AudioData audioData = soundLibrary?.GetAudioData(soundName);
        if (audioData?.clip == null)
        {
            Debug.LogWarning($"Could not find audio data for sound: {soundName}");
            return null;
        }

        return PlaySound(audioData, position);
    }

    public AudioSource PlaySound(AudioData audioData, Vector3 position = default)
    {
        if (audioData?.clip == null) return null;

        // Limit simultaneous sounds if necessary
        if (playingSounds.Count >= maxSimultaneousSounds)
        {
            StopOldestSound();
        }

        AudioSource source = audioSourcePool.GetAudioSource();
        ConfigureAudioSource(source, audioData, position);

        source.Play();

        // Track playing sound
        PlayingSoundInfo soundInfo = new PlayingSoundInfo
        {
            source = source,
            startTime = Time.time,
            soundName = audioData.name
        };
        playingSounds.Add(soundInfo);

        StartCoroutine(ReturnSourceWhenFinished(source, audioData.clip.length));

        return source;
    }

    private void ConfigureAudioSource(AudioSource source, AudioData audioData, Vector3 position)
    {
        source.clip = audioData.clip;
        source.volume = audioData.volume;
        source.pitch = audioData.pitch;
        source.loop = audioData.loop;
        source.outputAudioMixerGroup = audioData.mixerGroup ?? masterMixerGroup;

        if (audioData.is3D)
        {
            source.spatialBlend = 1f;
            source.maxDistance = audioData.maxDistance;
            source.rolloffMode = audioData.rolloffMode;
            source.transform.position = position;
        }
        else
        {
            source.spatialBlend = 0f;
        }
    }

    private IEnumerator ReturnSourceWhenFinished(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength + 0.1f);

        if (source != null && !source.isPlaying)
        {
            ReturnAudioSourceToPool(source);
        }
    }

    private void ReturnAudioSourceToPool(AudioSource source)
    {
        // Remove from playing sounds list
        for (int i = playingSounds.Count - 1; i >= 0; i--)
        {
            if (playingSounds[i].source == source)
            {
                playingSounds.RemoveAt(i);
                break;
            }
        }

        audioSourcePool?.ReturnAudioSource(source);
    }

    private void StopOldestSound()
    {
        if (playingSounds.Count > 0)
        {
            AudioSource oldestSource = playingSounds[0].source;
            if (oldestSource != null)
            {
                oldestSource.Stop();
                ReturnAudioSourceToPool(oldestSource);
            }
        }
    }

    public void StopAllSounds()
    {
        for (int i = playingSounds.Count - 1; i >= 0; i--)
        {
            if (playingSounds[i].source != null)
            {
                playingSounds[i].source.Stop();
                ReturnAudioSourceToPool(playingSounds[i].source);
            }
        }
        playingSounds.Clear();
    }

    void Start()
    {
        audioClipsMenuSource = UIManager.i.audioClipsMenu.GetComponent<AudioSource>();
        if (audioClipsMenuSource == null)
            audioClipsMenuSource = UIManager.i.audioClipsMenu.AddComponent<AudioSource>();

        StartCoroutine(AudioTimingCoroutine());
        StartCoroutine(AudioCleanupCoroutine());
        StartCoroutine(NonPlayingUpdateCoroutine());
    }

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

    private IEnumerator AudioTimingCoroutine()
    {
        while (true)
        {
            if (globalPlay)
            {
                time = AudioSettings.dspTime;
                double sixteenthNote = 60.0 / (bpm * 4);

                if (time + sixteenthNote > nextEventTime)
                {
                    foreach (Sequence sequence in sequences)
                    {
                        int localBeatIndex = beatIndex % sequence.sequenceLengthMultiplier;
                        if (localBeatIndex == 0)
                        {
                            sequence.Schedule(beatIndex, nextEventTime);
                        }
                    }

                    nextEventTime += sixteenthNote;
                    beatIndex++;

                    TimeEvent te = new TimeEvent(nextEventTime, EveryStepAction, beatIndex);
                    StartCoroutine(CheckEventRoutine(te));
                }

                yield return null; // Run every frame when playing
            }
            else
            {
                yield return new WaitForSeconds(0.1f); // Check less frequently when not playing
            }
        }
    }

    private IEnumerator AudioCleanupCoroutine()
    {
        while (true)
        {
            CleanupFinishedSounds();
            yield return new WaitForSeconds(0.1f); // Clean up every 100ms instead of every frame
        }
    }

    private void CleanupFinishedSounds()
    {
        for (int i = playingSounds.Count - 1; i >= 0; i--)
        {
            if (playingSounds[i].source == null || !playingSounds[i].source.isPlaying)
            {
                if (playingSounds[i].source != null)
                {
                    ReturnAudioSourceToPool(playingSounds[i].source);
                }
                else
                {
                    playingSounds.RemoveAt(i);
                }
            }
        }
    }

    private int lateUpdateCount = 0;

    private IEnumerator NonPlayingUpdateCoroutine()
    {
        while (true)
        {
            if (!globalPlay)
            {
                EveryStepAction(AudioSettings.dspTime, beatIndex);
                yield return new WaitForSeconds(1.0f); // Update every second when not playing
            }
            else
            {
                yield return new WaitForSeconds(0.1f); // Check state more frequently
            }
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

        for (int i = sequencers.Count - 1; i >= 0; i--)
        {
            if (sequencers[i] != null)
            {
                sequencers[i].OnEveryStep(x);
            }
            else
            {
                sequencers.RemoveAt(i);
            }
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

    public void SwapAudioClip(AudioClip _clip, CreatureFamily _creatureFamily){
        if(_creatureFamily.creatureData.creatureMemberCount != 1){
            Debug.LogError("SwapAudioClip: CreatureMemberCount is not 1");
            return;
        }
        //swap in CreatureData
        _creatureFamily.creatureData.audioClips.Clear();
        _creatureFamily.creatureData.audioClips.Add(_clip);
        //swap AudioSource and Clip in Sequences
        foreach(Sequence sequence in _creatureFamily.sequencer.sequences){
            sequence.clip = _clip;
            foreach(AudioSource source in sequence.sources){
                source.clip = _clip;
            }
        }

        //swap in CreatureMember
        foreach(CreatureMember member in _creatureFamily.creatureMembers){
            if(member != null) {
                member.clip = _clip;
            }
        }
    }

    public void PlayAudioClip(AudioClip _clip){

        audioClipsMenuSource.clip = _clip;
        audioClipsMenuSource.Play();
    }

}
