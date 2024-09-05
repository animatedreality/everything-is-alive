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
    public void InitiateCreature(GameObject model, AudioClip clip, Vector3 originalGlobalScale){
        //offset local scale from all the parent scale
        Vector3 totalParentScale = GetTotalParentScale();
        Vector3 correctedLocalScale = new Vector3(
            originalGlobalScale.x / totalParentScale.x,
            originalGlobalScale.y / totalParentScale.y,
            originalGlobalScale.z / totalParentScale.z
        );
        model.transform.localScale = correctedLocalScale;
        model.transform.parent = modelContainer;
        model.AddComponent<BoxCollider>();
        model.GetComponent<BoxCollider>().isTrigger = false;
        // model.AddComponent<MaterialPropertyBlockEditor>();
        scaleOnPlay = model.AddComponent<ScaleOnPlay>();
        //append the ScaleUp method to the OnPlay event
        instrument.OnPlay.AddListener((int value) => scaleOnPlay.ScaleUp());
        instrument.clip = clip;
    }

    public void UpdateAudioClip(AudioClip clip){
        instrument.clip = clip;
    }

    Vector3 GetTotalParentScaleBackup()
    {
        // If there is no parent, return Vector3.one (identity scale)
        if (transform.parent == null)
        {
            return Vector3.one;
        }

        // Multiply the parent's global scale by the parent's parent's scale, recursively
        return Vector3.Scale(transform.parent.lossyScale, GetTotalParentScale());
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
