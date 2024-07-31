using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    public class Global : MonoBehaviour
    {
        public static Global instance;

        public float bpm = 120f;

        private double time;

        private double nextEventTime;

        private bool globalPlay = false;


        public float GetSampleRate()
        {
            return AudioSettings.outputSampleRate;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            nextEventTime = AudioSettings.dspTime + 0.4;
        }

        public void Update()
        {
            time = AudioSettings.dspTime;
            double sixteenthNote = 60.0 / (bpm * 4);
            if (globalPlay)
            {
                if (time + sixteenthNote > nextEventTime)
                {

                }
            }
        }
    }
}