using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Audio;
public class AnimatewithAudio : MonoBehaviour
{
    [Header("Only Assign Creature with InstrumentSample")]
    public Creature thisCreature;
    public AudioSource audioSource;
    [HideInInspector]
    public AudioClip[] sounds; // Set the array size and the sounds in the Inspector
    private float[] freqData;
    private int nSamples = 256;
    private float fMax = 24000f;
    //public List<AudioSource> audioSources;

    private float volume = 40f;
    private float frqLow = 200f;
    private float frqHigh = 800f;

    public float voiceBandVol = 0f;
    public float volumeMultiplier = 100f;
    public float frequencyMultiplier = 0.5f;
    public float rmsValue = 0f;

    private float[] samples;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    public FloatEvent onVoiceBandVolChange;
    public FloatEvent onVolumeChange;

    void Start()
    {

    }

    public void AssignAudioSource(AudioSource source){
        audioSource = source;
        freqData = new float[nSamples];
        if (onVoiceBandVolChange == null)
            onVoiceBandVolChange = new FloatEvent();
        samples = new float[nSamples];
        if (onVolumeChange == null)
            onVolumeChange = new FloatEvent();
    }

    // Update is called once per frame
    void Update()
    {
        //targetObj.transform.localScale = new Vector3(scale, scale, scale);
        //invoke a unity event here passing the voiceBandVol
        //onVoiceBandVolChange.Invoke(voiceBandVol);
        onVoiceBandVolChange.Invoke(BandVol(audioSource, frqLow, frqHigh) * volume * frequencyMultiplier + GetVolume(audioSource) * volumeMultiplier);
    }



    float BandVol(AudioSource source, float fLow, float fHigh)
    {
        fLow = Mathf.Clamp(fLow, 20, fMax); // Limit low...
        fHigh = Mathf.Clamp(fHigh, fLow, fMax); // and high frequencies
        // Get spectrum: freqData[n] = vol of frequency n * fMax / nSamples
        source.GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);
        int n1 = Mathf.FloorToInt(fLow * nSamples / fMax);
        int n2 = Mathf.FloorToInt(fHigh * nSamples / fMax);
        float sum = 0;
        // Average the volumes of frequencies fLow to fHigh
        for (int i = n1; i <= n2; i++)
        {
            sum += freqData[i];
        }
        return sum / (n2 - n1 + 1);
    }

    float GetVolume(AudioSource source)
    {
        source.GetOutputData(samples, 0);
        float sum = 0;
        for (int i = 0; i < nSamples; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / nSamples);
        return rmsValue;
    }
}
