using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    public static class Util
    {

        static string[] noteNames = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };


        public static float GetSampleRate()
        {
            return AudioSettings.outputSampleRate;
        }


        public static string GetNoteName_FromMidiNum(int midiNum)
        {

            int note = midiNum % 12;
            int octave = midiNum / 12 - 1;
            return noteNames[note] + octave;
        }

        public static float GetFrequency_FromMidiNum(int midiNum)
        {
            return 440f * Mathf.Pow(2f, (midiNum - 69) / 12f);
        }

        public static int GetMidiNum_FromNoteName(string noteName)
        {
            //separate octave from note name as int
            int octave = int.Parse(noteName.Substring(noteName.Length - 1, 1));
            //get note name without octave
            string note = noteName.Substring(0, noteName.Length - 1);
            //get index of note in noteNames array
            int noteIndex = System.Array.IndexOf(noteNames, note);
            //calculate midi number
            int midiNum = octave * 12 + noteIndex + 12;
            return midiNum;
        }

        public static float GetFrequency_FromNoteName(string noteName)
        {
            return GetFrequency_FromMidiNum(GetMidiNum_FromNoteName(noteName));
        }

        public static int GetMidiNum_FromFrequency(float frequency)
        {
            return Mathf.RoundToInt(69 + 12 * Mathf.Log(frequency / 440f) / Mathf.Log(2));
        }

        public static string GetNoteName_FromFrequency(float frequency)
        {
            return GetNoteName_FromMidiNum(GetMidiNum_FromFrequency(frequency));
        }

        public static Complex[] FFT_GetFreqArr(float[] data, int size = -1)
        {
            if (size == -1 || size > data.Length)
            {
                size = Mathf.ClosestPowerOfTwo(data.Length);
                int whileCount = 0;
                while (size > data.Length && whileCount < 100)
                {
                    whileCount++;
                    size /= 2;
                } //<-- shrink chunkSize to be close to the index size
                if (whileCount >= 100)
                {
                    Debug.Log("while loop count exceeded 100");
                }
            }
            Complex[] complexSlice = new Complex[size];
            for (int j = 0; j < size; j++)
            {
                complexSlice[j] = new Complex(data[j], 0);
            }
            return FFT.CalculateFFT(complexSlice, false); //<-- only use half the array!!!!! important.. but im not going to resize it here for the sake of speed
        }

        public static List<float> GetLoudestFrequencies(float[] data, int numFreq)
        {
            float sampleRate = GetSampleRate();
            List<float> loudestFreqs = new List<float>();
            Complex[] fft = FFT_GetFreqArr(data);
            //get first half of fft
            Vector2[] fftWithIndex = new Vector2[fft.Length / 2];
            for (int i = 0; i < fftWithIndex.Length; i++)
            {
                fftWithIndex[i] = new Vector2(i, fft[i].fMagnitude);
            }
            //sort fftWithIndex by fMagnitude
            System.Array.Sort(fftWithIndex, (a, b) => b.y.CompareTo(a.y));
            //get the 7 loudest frequencies
            if (numFreq > fftWithIndex.Length) numFreq = fftWithIndex.Length;
            int w = 0;
            while (w < numFreq) //some of the frequencies are too low to be audible (sometimes even 0 oddly) so we only take the ones above 10hz
            {
                float freq = fftWithIndex[w].x * (sampleRate / (float)fft.Length);
                if (freq > 10f)
                {
                    loudestFreqs.Add(freq);
                }
                w++;
            }
            return loudestFreqs;
        }

        public static float GetFundamentalFreqFromLoudestFrequencies(float[] data, List<float> loudestFreqList)
        {
            float loudestFreq = loudestFreqList[0];
            //check for the 6:5 ratio to the loudestFreq to see if it is a minor chord
            // bool minor = false;
            // float threeSemitonesAbove = loudestFreq * Mathf.Pow(2, 3 / 12);
            // float justRatio = loudestFreq * 6f / 5f;
            // for (int i = 1; i < loudestFreqList.Count; i++)
            // {
            //     // if loudestFreq * 6/5 approximates loudestFreqArr[i] then it is a minor chord
            //     if (Mathf.Approximately(justRatio, loudestFreqList[i]) || Mathf.Approximately(threeSemitonesAbove, loudestFreqList[i]))
            //     {
            //         loudestFreq = loudestFreqList[i];
            //         break;
            //     }
            // }
            return loudestFreq;
        }

        public static float GetFundamentalFromData(float[] data)
        {
            List<float> loudestFreqs = GetLoudestFrequencies(data, 8);
            return GetFundamentalFreqFromLoudestFrequencies(data, loudestFreqs);
        }

        public static float GetFundamentalFromClip(AudioClip clip)
        {
            float[] data = new float[clip.samples * clip.channels];
            clip.GetData(data, 0);
            return GetFundamentalFromData(data);
        }

        public static string AutoID(int length = -1)
        {
            string s = System.Guid.NewGuid().ToString();
            if (length > 0)
            {
                s = s.Substring(0, length);
            }
            return s;
        }


    }
}