using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    //create static instance
    [Header("3D")]
    public static PrefabManager instance;
    public GameObject instGroupPrefab;
    public GameObject creatureGroupPrefab;
    public GameObject creatureTemplatePrefab;

    [Header("UI")]
    public GameObject buttonCreaturePrefab;
    public GameObject buttonAudioPrefab;

    public GameObject instrumentPrefab;
    public GameObject buttonAddPrefab;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
