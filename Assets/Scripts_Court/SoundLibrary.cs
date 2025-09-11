using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioData audioData;
    }

    public List<SoundEffect> soundEffects = new List<SoundEffect>();

    public AudioData GetAudioData(string soundName)
    {
        foreach (var sound in soundEffects)
        {
            if (sound.name == soundName)
            {
                return sound.audioData;
            }
        }

        Debug.LogWarning($"Sound '{soundName}' not found in library");
        return null;
    }
}
