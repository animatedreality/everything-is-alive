using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    [SerializeField] private int poolSize = 20;
    [SerializeField] private GameObject audioSourcePrefab;

    private Queue<AudioSource> availableSources = new Queue<AudioSource>();
    private List<AudioSource> allSources = new List<AudioSource>();

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        GameObject sourceObject = new GameObject($"PooledAudioSource_{allSources.Count}");
        sourceObject.transform.SetParent(transform);
        AudioSource source = sourceObject.AddComponent<AudioSource>();

        allSources.Add(source);
        availableSources.Enqueue(source);

        return source;
    }

    public AudioSource GetAudioSource()
    {
        if (availableSources.Count > 0)
        {
            return availableSources.Dequeue();
        }

        return CreateNewAudioSource();
    }

    public void ReturnAudioSource(AudioSource source)
    {
        if (source != null && allSources.Contains(source))
        {
            source.Stop();
            source.clip = null;
            source.volume = 1f;
            source.pitch = 1f;
            availableSources.Enqueue(source);
        }
    }
}
