using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Audio;
using UnityEngine.Events;

public class CreatureModelSwapper : MonoBehaviour
{
    public Transform modelContainer;
    public Instrument instrument;
    ScaleOnPlay scaleOnPlay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //when loading this creature from the menu, customize all the elements here
    public void InitiateCreature(GameObject model, AudioClip clip = null){
        model.transform.parent = modelContainer;
        model.AddComponent<BoxCollider>();
        model.GetComponent<BoxCollider>().isTrigger = false;
        // model.AddComponent<MaterialPropertyBlockEditor>();
        scaleOnPlay = model.AddComponent<ScaleOnPlay>();
        if(clip != null){
            UpdateAudioClip(clip);
        }
        //append the ScaleUp method to the OnPlay event
        instrument.OnPlay.AddListener((int value) => scaleOnPlay.ScaleUp());
    }

    public void UpdateAudioClip(AudioClip clip){
        Debug.Log("UpdateAudioClip" + clip.name);
        instrument.clip = clip;
        instrument.UpdateAudioClip(clip);
    }
    Vector3 GetTotalParentScale()
    {
        Vector3 totalScale = Vector3.one;
        Transform currentTransform = transform;

        while (currentTransform.parent != null)
        {
            Debug.Log("Get Parent Scale" + currentTransform.name);
            currentTransform = currentTransform.parent;
            totalScale = Vector3.Scale(totalScale, currentTransform.localScale);
        }

        return totalScale;
    }
}
