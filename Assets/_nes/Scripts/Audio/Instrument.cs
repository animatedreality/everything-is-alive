using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    public class Instrument : MonoBehaviour
    {
        public string instrumentName, id;
        public List<AudioSource> sources = new List<AudioSource>();

        public AudioClip clip;

        public Note note;

        int loopLength = 16;

        float pitch, pan, volume;

        private float ogClipFundamentalFreq;

        List<int> sequence = new List<int>();

        public Instrument(string name, Note note, AudioClip clip)
        {
            this.instrumentName = name;
            this.note = note;
            this.clip = clip;
            this.id = Util.AutoID();
            this.ogClipFundamentalFreq = Util.GetFundamentalFromClip(clip);
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
            AudioSource source = GetAvailableSource();
            source.PlayScheduled(time);
        }

    }
}