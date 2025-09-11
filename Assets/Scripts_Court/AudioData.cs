using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/Audio Data")]
public class AudioData : ScriptableObject
{
    [Header("Audio Settings")]
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop = false;
    public AudioMixerGroup mixerGroup;

    [Header("3D Audio Settings")]
    public bool is3D = false;
    [Range(0f, 1000f)] public float maxDistance = 100f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
}
