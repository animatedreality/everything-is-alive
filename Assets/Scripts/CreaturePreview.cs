using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturePreview : MonoBehaviour
{
    public List<Renderer> renderers = new List<Renderer>();
    public Material previewMaterial;
    // Start is called before the first frame update
    void Start()
    {
        previewMaterial = Audio.Global.instance.previewMaterial;
        renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        SetMaterialPreview();

    }

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();
    }

    public void LookAtPlayer()
    {
        AudioListener audioListener = FindObjectOfType<AudioListener>();
        if (audioListener != null)
        {
            Vector3 dirToListener = audioListener.transform.position - transform.position;
            Vector3 horizontalDirection = Vector3.ProjectOnPlane(dirToListener, Vector3.up);
            Quaternion rotation = Quaternion.LookRotation(horizontalDirection);
            rotation *= Quaternion.Euler(0, 180, 0);
            transform.rotation = rotation;
        }
    }

    public void SetMaterialPreview(){
        Debug.Log("Setting material preview");
        foreach(Renderer renderer in renderers){
            Material[] sharedMats = renderer.sharedMaterials;
            for (int i = 0; i < sharedMats.Length; i++) {
                sharedMats[i] = previewMaterial;
            }
            renderer.sharedMaterials = sharedMats;
        }
    }

    public void SetVisuals(string _mode){
    }
}
