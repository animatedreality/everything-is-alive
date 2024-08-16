using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N_RemoveGlobalMesh : MonoBehaviour
{
    private string nameToRemove = "GLOBAL_MESH";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if we can find a gameobject in scene salled GLOBAL_MESH, disable it
        GameObject[] globalMeshes = GameObject.FindGameObjectsWithTag(nameToRemove);
        foreach (GameObject gm in globalMeshes)
        {
            gm.SetActive(false);
        }
    }
}
