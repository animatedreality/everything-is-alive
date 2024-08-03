using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class Instrument : MonoBehaviour
    {
        [HideInInspector]
        public string instrumentName, id;
        public List<AudioSource> sources = new List<AudioSource>();

        public AudioClip clip;

        public Note note;

        private int loopLength;

        float pitch = 1f, pan = 0f, volume = 1f;

        private float ogClipFundamentalFreq;

        List<int> sequence = new List<int>();

        List<Dot> dots = new List<Dot>();

        public GameObject dotPrefab;

        void Start()
        {
            this.id = Util.AutoID();
            InstGroup instGroup = GetComponentInParent<InstGroup>();
            if (instGroup != null)
            {
                SetInstGroup(instGroup);
            }
        }

        public void SetInstGroup(InstGroup instGroup)
        {
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
                if (sequence[i] == globalBeatIndex % loopLength)
                {
                    PlayAtTime(nextEventTime);
                }
            }
        }

        public void PlayAtTime(double time)
        {
            if (time <= AudioSettings.dspTime)
            {
                return;
            }
            AudioSource source = GetAvailableSource();
            source.PlayScheduled(time);
        }

    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace Audio
// {
//     public class Instrument : MonoBehaviour
//     {
//         [HideInInspector]
//         public string instrumentName, id;

//         public AudioSource source;
//         public AudioClip clip;

//         public Note note;

//         int loopLength = 16;

//         float pitch = 1f, pan = 0f, volume = 1f;

//         private float ogClipFundamentalFreq;

//         List<int> sequence = new List<int>();

//         public GameObject dotPrefab;

//         void Start()
//         {
//             this.id = Util.AutoID();
//             for (int i = 0; i < loopLength; i++)
//             {
//                 GameObject dot = Instantiate(dotPrefab, transform);
//                 Dot dotScript = dot.GetComponent<Dot>();
//                 dotScript.instrument = this;
//             }
//             InstGroup instGroup = transform.parent.GetComponent<InstGroup>();
//             if (instGroup != null)
//             {
//                 instGroup.AddInstrument(this);
//             }
//             if (source == null)
//             {
//                 source = gameObject.AddComponent<AudioSource>();
//                 source.playOnAwake = false;
//                 source.loop = false;
//                 SyncSourceVariables();
//             }
//         }

//         public void SetInstGroup(InstGroup instGroup)
//         {
//             instGroup.AddInstrument(this);
//         }

//         public void Set(string name, Note note, AudioClip clip)
//         {
//             this.instrumentName = name;
//             this.note = note;
//             this.clip = clip;
//             this.ogClipFundamentalFreq = Util.GetFundamentalFromClip(clip);
//             this.id = Util.AutoID();
//             SyncSourceVariables();
//         }

//         public void AddToSequence(int index)
//         {
//             if (!sequence.Contains(index))
//             {
//                 sequence.Add(index);
//             }
//         }

//         public void RemoveFromSequence(int index)
//         {
//             if (sequence.Contains(index))
//             {
//                 sequence.Remove(index);
//             }
//         }

//         public void Play()
//         {
//             if (!source.isPlaying)
//             {
//                 source.Play();
//             }
//         }

//         public void Stop()
//         {
//             source.Stop();
//         }

//         private void SyncSourceVariables()
//         {
//             source.pitch = pitch;
//             source.panStereo = pan;
//             source.volume = volume;
//             source.clip = clip;
//         }

//         public void SetPan(float pan)
//         {
//             this.pan = pan;
//             source.panStereo = pan;
//         }

//         public void SetNote(Note note)
//         {
//             this.note = note;
//             float ratio = note.frequency / ogClipFundamentalFreq;
//             SetPitch(ratio);
//         }

//         public void SetPitch(float pitch)
//         {
//             this.pitch = pitch;
//             source.pitch = pitch;
//         }

//         public void SetVolume(float volume)
//         {
//             this.volume = volume;
//             source.volume = volume;
//         }

//         public void SetClip(AudioClip clip)
//         {
//             this.clip = clip;
//             source.clip = clip;
//         }

//         public void Schedule(int globalBeatIndex, double nextEventTime)
//         {
//             for (int i = 0; i < sequence.Count; i++)
//             {
//                 if (sequence[i] == globalBeatIndex % loopLength)
//                 {
//                     PlayAtTime(nextEventTime);
//                 }
//             }
//         }

//         public void PlayAtTime(double time)
//         {
//             Debug.Log("Playing at time: " + time);
//             source.PlayScheduled(time);
//         }
//     }
// }
