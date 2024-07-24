using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//require audiosource component
[RequireComponent(typeof(AudioSource))]
public class N_PlaySoundOnTouch : MonoBehaviour
{
    public AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio()
    {
        audio.Play();
    }
}
