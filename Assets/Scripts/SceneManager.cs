using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SceneManager : MonoBehaviour
{
    //this is what MenuManager dreamed to be
    [Tooltip("SceneManager controls order of everything, including Game and Audio Manager")]
    public static SceneManager i;
    private void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [Header("InGame UI")]
    public GameObject assetMenuDefault;
    public GameObject assetMenuMona;
    [Header("Prefabs")]
    public GameObject buttonPrefab;

    void Start()
    {
        AudioManager.i.Initialize();
        CreatureManager.i.Initialize();
        InitializeAssetMenuDefault();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializeAssetMenuDefault(){
        foreach(CreatureData creatureData in CreatureManager.i.creatureDataList){
            GameObject button = Instantiate(buttonPrefab, assetMenuDefault.transform);
            button.GetComponent<Image>().sprite = creatureData.sprite;
            button.name = creatureData.name;
        }
    }
}
