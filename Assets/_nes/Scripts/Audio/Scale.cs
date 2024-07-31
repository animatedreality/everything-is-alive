using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    public class Scale : MonoBehaviour
    {
        public static Scale instance;

        public Note tonic;

        public List<float> ratios = new List<float>();

        public void SetScale(Note tonic, List<float> ratios)
        {
            this.tonic = tonic;
            this.ratios = ratios;
        }

        public void SetScaleFromNoteNames(List<string> noteNames)
        {
            List<float> ratios = new List<float>();
            Note tonic = new Note(noteNames[0]);
            foreach (string noteName in noteNames)
            {
                Note note = new Note(noteName);
                float ratio = note.frequency / tonic.frequency;
                ratios.Add(ratio);
            }
            SetScale(tonic, ratios);
        }

        public void SetScaleFromMidiNums(List<int> midiNums)
        {
            List<float> ratios = new List<float>();
            Note tonic = new Note(midiNums[0]);
            foreach (int midiNum in midiNums)
            {
                Note note = new Note(midiNum);
                float ratio = note.frequency / tonic.frequency;
                ratios.Add(ratio);
            }
            SetScale(tonic, ratios);
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
    }
}