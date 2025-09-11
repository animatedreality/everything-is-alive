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
    private int nSamples = 128; //changed from 256 to 128
    private float fMax = 24000f;

    // Cache arrays and reuse them
    private static float[] sharedFreqData = new float[128];
    private static float[] sharedSamples = new float[128];

    private float volume = 40f;
    private float frqLow = 200f;
    private float frqHigh = 800f;

    public float voiceBandVol = 0f;
    public float volumeMultiplier = 100f;
    public float frequencyMultiplier = 0.5f;
    public float rmsValue = 0f;

    private float[] samples;

    private Transform cachedTransform;
    private AudioSource cachedAudioSource;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    public FloatEvent onVoiceBandVolChange;
    public FloatEvent onVolumeChange;

    void Awake()
    {
        // Cache references once
        cachedTransform = transform;
        cachedAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(AudioAnalysisCoroutine());
    }

    public void AssignAudioSource(AudioSource source)
    {
        audioSource = source;
        freqData = sharedFreqData;
        if (onVoiceBandVolChange == null)
            onVoiceBandVolChange = new FloatEvent();
        samples = sharedSamples;
        if (onVolumeChange == null)
            onVolumeChange = new FloatEvent();
    }

    private IEnumerator AudioAnalysisCoroutine()
    {
        while (true)
        {
            if (audioSource != null && audioSource.isPlaying && audioSource.clip != null)
            {
                // Perform expensive audio analysis
                onVoiceBandVolChange.Invoke(BandVol(audioSource, frqLow, frqHigh) * volume * frequencyMultiplier + GetVolume(audioSource) * volumeMultiplier);
            }
            yield return new WaitForSeconds(0.066f); // Update at 30Hz instead of 60Hz -> Changed to 15Hz by Court
        }
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
