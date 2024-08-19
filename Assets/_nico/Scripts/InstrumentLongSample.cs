using UnityEngine;
using Audio;


namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class InstrumentLongSample : Instrument
    {
        public bool isPlaying = false;
        public AudioSource longSampleAudioSource;
        public AnimatewithAudio animatewithAudio;

        new void Start()
        {
            base.Start();
            InitializeAudioSource();
            //ToggleSound();
            PlayLongSample();
        }

        public void ToggleSound()
        {
            Debug.Log("Toggle sound of Long Sample Class in START");
            isPlaying = !isPlaying;
            if (isPlaying)
            {
                Stop();
            }
            else
            {
                PlayLongSample();
            }
            
        }

        private void PlayLongSample()
        {
            Debug.Log("instrumentGroup is " + instGroup);
            Debug.Log("first instrumentGroup is " + firstInstGroup);
            AudioSource source = GetAvailableSource();
            source.loop = true;
            source.Play();
        }

        public new void Stop()
        {
            base.Stop();
            isPlaying = false;
        }

        public override AudioSource GetAvailableSource()
        {
            SyncSourceVariables(longSampleAudioSource);
            return longSampleAudioSource;
        }

        private void InitializeAudioSource()
        {
            longSampleAudioSource = GetComponent<AudioSource>();
            longSampleAudioSource.playOnAwake = false;
            longSampleAudioSource.loop = true;
            longSampleAudioSource.volume = 0.75f;
            longSampleAudioSource.spatialBlend = 1f;
            SyncSourceVariables(longSampleAudioSource);
            sources.Add(longSampleAudioSource);
            if(animatewithAudio != null){
                animatewithAudio.AssignAudioSource(longSampleAudioSource);
            }
        }

        // Override methods that are not needed for this type of instrument
        public override void SetLoopLength(int length) { }
        public override void AddToSequence(int index) { }
        public override void RemoveFromSequence(int index) { }
        public override void Schedule(int globalBeatIndex, double nextEventTime) { }


    }
}