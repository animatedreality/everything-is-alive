using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    public class Instrument : MonoBehaviour
    {
        [HideInInspector]
        public string instrumentName, id;
        public List<AudioSource> sources = new List<AudioSource>();

        public AudioClip clip;

        public delegate void OnPlayDelegate(int x);

        public event OnPlayDelegate OnPlayEvent;

        public UnityEvent<int> OnPlay;

        public Note note;

        private int loopLength;

        float pitch = 1f, pan = 0f, volume = 1f;

        private float ogClipFundamentalFreq;

        List<int> sequence = new List<int>();

        List<Dot> dots = new List<Dot>();

        public GameObject dotPrefab;

        InstGroup instGroup;

        void Start()
        {
            this.id = Util.AutoID();
            InstGroup instGroup = GetComponentInParent<InstGroup>();
            if (instGroup != null)
            {
                SetInstGroup(instGroup);
            }
        }

        private void Reset()
        {
            if (OnPlay == null)
            {
                OnPlay = new UnityEvent<int>();
            }
        }

        public void SetInstGroup(InstGroup instGroup)
        {
            this.instGroup = instGroup;
            instGroup.AddInstrument(this);
        }

        public void SetLoopLength(int length)
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

        public void AddToSequence(int index)
        {
            if (!sequence.Contains(index))
            {
                sequence.Add(index);
            }
        }

        public void RemoveFromSequence(int index)
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

        public AudioSource GetAvailableSource()
        {
            foreach (AudioSource source in sources)
            {
                Debug.Log("source.isPlaying: " + source.isPlaying);
                if (!source.isPlaying)
                {
                    SyncSourceVariables(source);
                    return source;
                }
            }
            return CreateNewSource();
        }

        private void SyncSourceVariables(AudioSource source)
        {
            source.pitch = pitch;
            source.panStereo = pan;
            source.volume = volume;
            source.clip = clip;
        }

        public AudioSource CreateNewSource()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
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

        public void SetClip(AudioClip clip)
        {
            this.clip = clip;
            foreach (AudioSource source in sources)
            {
                source.clip = clip;
            }
        }

        public void Schedule(int globalBeatIndex, double nextEventTime)
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
            OnPlay?.Invoke(localBeatIndex);
            OnPlayEvent?.Invoke(localBeatIndex);
        }

    }
}