using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager : MonoBehaviour
{
    public static CreatureManager i;
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

    public bool isInGame = true;

    public float creatureScaleMultiplier = 3;
    //Load all CreatureData from Resources/CreatureData
    public List<CreatureData> creatureDataList;
    public GameObject creatureFamilyPrefab;
    public Transform creatureSpawnPoint;

    //assigned when the creature's UIButtonContainer SelectButton() is called
    public CreatureData selectedCreatureData;

    //tracks which creature is currently being selectd
    public CreatureFamily selectedCreatureFamily;

    [Header("Instantiating Creatures")]
    public CreatureData creatureDataTemplate;
    public GameObject creatureMeshPrefab;
    public CreatureFamily tempMonaCreatureFamily;//temporarily create a new creatureFamily when browsing Mona Models

    // Start is called before the first frame update
    public void Initialize()
    {
        creatureDataList = new List<CreatureData>(Resources.LoadAll<CreatureData>("CreatureData"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCreature(){
        //if globalPlay is false, start the game
        if(!AudioManager.i.globalPlay){
            AudioManager.i.Play();
        }
        //Spawn Creature if there has been one selected
        if(selectedCreatureData != null && isInGame){
            GameObject creature = Instantiate(creatureFamilyPrefab);
            creature.transform.position = creatureSpawnPoint.position;
            Debug.Log("SpawnCreature");
        }
    }

    public void SelectCreatureFamily(CreatureFamily _creatureFamily){
        if(selectedCreatureFamily != null){
            selectedCreatureFamily.OnDeselect();
        }
        selectedCreatureFamily = _creatureFamily;
        Debug.Log("Selected Creature Family: " + selectedCreatureFamily.name);
    }

    public void CreateTempMonaCreature(GameObject _model){
        //implement creatureMesh
        GameObject newCreatureMesh = Instantiate(creatureMeshPrefab);

        //fill in this creatureMesh with _model
        _model.transform.parent = newCreatureMesh.transform.Find("MeshContainer");
        _model.transform.localPosition = Vector3.zero;

        //implement creatureData
        CreatureData newCreatureData = ScriptableObject.CreateInstance<CreatureData>();
        newCreatureData.name = _model.name;
        newCreatureData.prefab = newCreatureMesh;
        newCreatureData.sprite = creatureDataTemplate.sprite;//set this from Mona later
        newCreatureData.audioClips = creatureDataTemplate.audioClips;
        newCreatureData.creatureMemberCount = 1;
        newCreatureData.creatureType = CreatureData.CreatureType.Drum;
        newCreatureData.sequenceLengthMultiplier = 1;
        newCreatureData.sequenceLength = 16;

        selectedCreatureData = newCreatureData;

        //implement creatureFamily
        GameObject newCreatureFamily = Instantiate(creatureFamilyPrefab);
        newCreatureFamily.name = _model.name;
        tempMonaCreatureFamily = newCreatureFamily.GetComponent<CreatureFamily>();
        //tempMonaCreatureFamily is automatically initialized with selectedCreatureData


    }

    public void SaveTempMonaCreature(){
        //save tempMonaCreatureData to persistent storage
        if(selectedCreatureData == tempMonaCreatureFamily.creatureData){
            H_PersistentStorage.SaveCreatureData(selectedCreatureData);
        }else{
            Debug.LogError("SaveTempMonaCreature: selectedCreatureData is not the same as tempMonaCreatureFamily.creatureData");
        }
        
        //also need to initialize CreatureManager to make sure the button to load this creature is enabled and the workflow exist
    }

    public void DeleteTempMonaCreature(){
        if(selectedCreatureData == tempMonaCreatureFamily.creatureData){
            selectedCreatureData = null;
        }
        Destroy(tempMonaCreatureFamily);
    }
}
