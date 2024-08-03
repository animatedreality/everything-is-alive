using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//require audiosource component
[RequireComponent(typeof(AudioSource))]
public class N_PlaySoundOnTouch : MonoBehaviour
{
    public AudioSource audio;
    public string targetStringContains = "Hand";
    public GameObject collidingObject;

    private bool isCooldown = false;
    private float cooldownTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    void Awake(){
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio()
    {
        //check if in cooldown period, if so, do nothing
        if (isCooldown) return;
        audio.Play();
        StartCoroutine(Cooldown());
    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.name.Contains(targetStringContains)){
            collidingObject = collision.gameObject;
            PlayAudio();
        }
    }

    private IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }
}
