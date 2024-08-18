using UnityEngine;
using Audio;

namespace Audio
{
    public class InstrumentLongSample : Instrument
    {
        public bool isPlaying = false;

        new void Start()
        {
            base.Start();
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

        // Override methods that are not needed for this type of instrument
        public override void SetLoopLength(int length) { }
        public override void AddToSequence(int index) { }
        public override void RemoveFromSequence(int index) { }
        public override void Schedule(int globalBeatIndex, double nextEventTime) { }


    }
}