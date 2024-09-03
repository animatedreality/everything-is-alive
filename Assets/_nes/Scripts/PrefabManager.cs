using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    //create static instance
    public static PrefabManager instance;
    public GameObject instGroupPrefab;
    public GameObject creatureGroupPrefab;

    public GameObject buttonCreaturePrefab;

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
