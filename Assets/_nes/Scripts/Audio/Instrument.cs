using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    public class Instrument : MonoBehaviour
    {
        public List<AudioSource> sources = new List<AudioSource>();
        public AudioClip clip;
        [HideInInspector]
        public string instrumentName, id;

        public delegate void OnPlayDelegate(int x);

        public event OnPlayDelegate OnPlayEvent;

        public UnityEvent<int> OnPlay;

        public Note note;

        public int loopLength;

        float pitch = 1f, pan = 0f, volume = 1f;

        private float ogClipFundamentalFreq;

        List<int> sequence = new List<int>();

        List<Dot> dots = new List<Dot>();

        public GameObject dotPrefab;

        public InstGroup instGroup;

        protected InstGroup firstInstGroup;

        protected void Start()
        {
            this.id = Util.AutoID();
            ResetInstGroup();
        }

        private void Reset()
        {
            if (OnPlay == null)
            {
                OnPlay = new UnityEvent<int>();
            }
        }

        public void ResetInstGroup()
        {
            instGroup = GetComponentInParent<InstGroup>();
            if (instGroup != null)
            {
                SetInstGroup(instGroup);
            }
        }

        public void FindOriginalInstGroup()
        {
            if (firstInstGroup != null)
            {
                SetInstGroup(firstInstGroup);
            }
            else
            {
                Debug.LogError("No firstInstGroup found for instrument " + instrumentName);
            }
        }

        public void SetInstGroup(InstGroup instGroup)
        {
            if (instGroup == null)
            {
                Debug.LogError("instGroup is null");
                return;
            }
            if (this.instGroup != null)
            {
                this.instGroup.RemoveInstrument(this);
            }
            if (firstInstGroup == null)
            {
                firstInstGroup = instGroup;
            }
            this.instGroup = instGroup;
            instGroup.AddInstrument(this);
        }

        public virtual void SetLoopLength(int length)
        {
            this.loopLength = length;
            if (loopLength > dots.Count)
            {
                int diff = loopLength - dots.Count;
                for (int i = 0; i < diff; i++)
                {
                    GameObject go = Instantiate(dotPrefab, transform);
                    Dot dot = go.GetComponent<Dot>();
                    dot.instrument = this;
                    dots.Add(dot);
                }
            }
            else if (loopLength < dots.Count)
            {
                int diff = dots.Count - loopLength;
                for (int i = 0; i < diff; i++)
                {
                    dots[dots.Count - 1].TurnOff();
                    Destroy(dots[dots.Count - 1].gameObject);
                    dots.RemoveAt(dots.Count - 1);
                }
            }
        }

        public int GetLoopLength()
        {
            return loopLength;
        }

        public void Set(string name, Note note, AudioClip clip)
        {
            this.instrumentName = name;
            this.note = note;
            this.clip = clip;
            this.ogClipFundamentalFreq = Util.GetFundamentalFromClip(clip);
            this.id = Util.AutoID();
        }

        public virtual void AddToSequence(int index)
        {
            if (!sequence.Contains(index))
            {
                sequence.Add(index);
            }
        }

        public virtual void RemoveFromSequence(int index)
        {
            if (sequence.Contains(index))
            {
                sequence.Remove(index);
            }
        }

        public void Play()
        {
            AudioSource source = GetAvailableSource();
            source.Play();
        }

        public void Stop()
        {
            foreach (AudioSource source in sources)
            {
                source.Stop();
            }
        }

        public virtual AudioSource GetAvailableSource()
        {
            foreach (AudioSource source in sources)
            {
                if (!source.isPlaying)
                {
                    SyncSourceVariables(source);
                    return source;
                }
            }
            return CreateNewSource();
        }

        public void UpdateAudioClip(AudioClip clip){
            Debug.Log("UpdateAudioClip in Instrument" + clip.name);
            this.clip = clip;
            foreach (AudioSource source in sources)
            {
                Debug.Log("Syncing audio clip in Instrument" + source.name);
                SyncSourceVariables(source);
            }
        }

        protected void SyncSourceVariables(AudioSource source)
        {
            source.pitch = pitch;
            source.panStereo = pan;
            source.volume = volume;
            source.clip = clip;
        }

        //NOT called in InstrumentSample
        public virtual AudioSource CreateNewSource()
        {
            Debug.Log("creating audio source from " + gameObject.name);
            Debug.Log("Creating audio source on " + firstInstGroup.gameObject.name);
            AudioSource source = firstInstGroup.gameObject.AddComponent<AudioSource>(); //just going to add the audio sources here so they stay localized to the creature (so the sound will emit from the creatures direction)
            source.clip = clip;
            source.playOnAwake = false;
            source.loop = false;
            source.volume = 0.75f; //we'll set this a bit lower than 1 for now to avoid clipping when instruments overlap, eventually we could have smart mixing set up like beat dj
            source.spatialBlend = 1f;
            SyncSourceVariables(source);
            sources.Add(source);
            
            return source;
        }

        public void SetPan(float pan)
        {
            this.pan = pan;
            foreach (AudioSource source in sources)
            {
                source.panStereo = pan;
            }
        }

        public void SetNote(Note note)
        {
            this.note = note;
            float ratio = note.frequency / ogClipFundamentalFreq;
            SetPitch(ratio);
        }

        public void SetPitch(float pitch)
        {
            this.pitch = pitch;
            foreach (AudioSource source in sources)
            {
                source.pitch = pitch;
            }
        }

        public void SetVolume(float volume)
        {
            this.volume = volume;
            foreach (AudioSource source in sources)
            {
                source.volume = volume;
            }
        }

        public void SetClip()
        {
            foreach (AudioSource source in sources)
            {
                source.clip = clip;
            }
        }

        public virtual void Schedule(int globalBeatIndex, double nextEventTime)
        {
            for (int i = 0; i < sequence.Count; i++)
            {
                int localBeatIndex = globalBeatIndex % loopLength;
                if (sequence[i] == localBeatIndex)
                {
                    PlayAtTime(nextEventTime, localBeatIndex);
                }
            }
        }

        public void PlayAtTime(double time, int localBeatIndex)
        {
            if (time <= AudioSettings.dspTime)
            {
                return;
            }
            AudioSource source = GetAvailableSource();
            source.PlayScheduled(time);
            StartCoroutine(OnPlayCoroutine(time, localBeatIndex));
        }

        IEnumerator OnPlayCoroutine(double time, int localBeatIndex)
        {
            yield return new WaitForSeconds((float)(time - AudioSettings.dspTime));
            if(instGroup.creature.currentVolume != 0){
                OnPlay?.Invoke(localBeatIndex);
                OnPlayEvent?.Invoke(localBeatIndex);
            }
            
        }

    }
}