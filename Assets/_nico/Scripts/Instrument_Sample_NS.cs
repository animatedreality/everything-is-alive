using UnityEngine;
using Audio;

namespace Audio
{
    public class Instrument_Sample_NS : Instrument
    {
        private bool isPlaying = false;

        public void ToggleSound()
        {
            if (isPlaying)
            {
                Stop();
            }
            else
            {
                PlayLongSample();
            }
            isPlaying = !isPlaying;
        }

        private void PlayLongSample()
        {
            AudioSource source = GetAvailableSource();
            source.loop = true;
            source.Play();
        }

        public new void Stop()
        {
            base.Stop();
            isPlaying = false;
        }

    }
}