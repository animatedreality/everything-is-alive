using UnityEngine;
using Audio;


namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class InstrumentSample : Instrument
    {
        [Header("Assign clip and sampleAudioSource in inspector please")]
        public bool isPlaying = false;
        public AudioSource sampleAudioSource;
        public AnimatewithAudio animatewithAudio;

        new void Start()
        {
            base.Start();
            InitializeAudioSource();
            //ToggleSound();
            PlaySample();
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
                PlaySample();
            }
            
        }

        private void PlaySample()
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
            SyncSourceVariables(sampleAudioSource);
            return sampleAudioSource;
        }

        private void InitializeAudioSource()
        {
            //sampleAudioSource = GetComponent<AudioSource>();
            sampleAudioSource.playOnAwake = false;
            sampleAudioSource.loop = true;
            sampleAudioSource.volume = 0.75f;
            sampleAudioSource.spatialBlend = 1f;
            SyncSourceVariables(sampleAudioSource);
            sources.Add(sampleAudioSource);
            if(animatewithAudio != null){
                animatewithAudio.AssignAudioSource(sampleAudioSource);
            }
        }

        public float GetSamplePlayTime(){
            if(sampleAudioSource == null) return 0;
            if(sampleAudioSource.clip == null) return 0;
            Debug.Log("sampleAudioSource.time: " + sampleAudioSource.time);
            Debug.Log("sampleAudioSource.clip.length: " + sampleAudioSource.clip.length);
            return (float) sampleAudioSource.time / (float) sampleAudioSource.clip.length;
        }

        // Override methods that are not needed for this type of instrument
        public override void SetLoopLength(int length) { }
        public override void AddToSequence(int index) { }
        public override void RemoveFromSequence(int index) { }
        public override void Schedule(int globalBeatIndex, double nextEventTime) { }


    }
}