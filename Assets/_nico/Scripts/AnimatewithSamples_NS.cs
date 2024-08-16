using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AnimatewithSamples_NS : MonoBehaviour
{
    public AudioClip[] sounds; // Set the array size and the sounds in the Inspector
    private float[] freqData;
    private int nSamples = 256;
    private float fMax = 24000f;
    public AudioSource audioSource;

    public float volume = 40f;
    public float frqLow = 200f;
    public float frqHigh = 800f;

    public float voiceBandVol = 0f;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    public FloatEvent onVoiceBandVolChange;

    void Start()
    {
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>(); // Get AudioSource component
        freqData = new float[nSamples];
        audioSource.Play();
        if (onVoiceBandVolChange == null)
            onVoiceBandVolChange = new FloatEvent();
    }

    // Update is called once per frame
    void Update()
    {
        voiceBandVol = BandVol(frqLow, frqHigh) * volume;
        //targetObj.transform.localScale = new Vector3(scale, scale, scale);
        //invoke a unity event here passing the voiceBandVol
        onVoiceBandVolChange.Invoke(voiceBandVol);
    }



    float BandVol(float fLow, float fHigh)
    {
        fLow = Mathf.Clamp(fLow, 20, fMax); // Limit low...
        fHigh = Mathf.Clamp(fHigh, fLow, fMax); // and high frequencies
        // Get spectrum: freqData[n] = vol of frequency n * fMax / nSamples
        audioSource.GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);
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
}
