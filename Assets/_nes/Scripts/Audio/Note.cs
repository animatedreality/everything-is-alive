using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    public class Note
    {
        public int midiNum;
        public string noteName;
        public float frequency;

        public Note(int midiNum)
        {
            this.midiNum = midiNum;
            this.noteName = Util.GetNoteName_FromMidiNum(midiNum);
            this.frequency = Util.GetFrequency_FromMidiNum(midiNum);
        }

        public Note(string noteName)
        {
            this.noteName = noteName;
            this.midiNum = Util.GetMidiNum_FromNoteName(noteName);
            this.frequency = Util.GetFrequency_FromNoteName(noteName);
        }

        public Note(float frequency)
        {
            this.frequency = frequency;
            this.midiNum = Util.GetMidiNum_FromFrequency(frequency);
            this.noteName = Util.GetNoteName_FromFrequency(frequency);
        }

    }
}