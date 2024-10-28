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
            creature.GetComponent<CreatureFamily>().Initialize(selectedCreatureData);
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

    public void CreateTempMonaCreature(GameObject _model, Vector3 _position){
        //implement creatureData
        CreatureData newCreatureData = ScriptableObject.CreateInstance<CreatureData>();
        newCreatureData.name = _model.name;
        newCreatureData.sprite = creatureDataTemplate.sprite;//set this from Mona later
        newCreatureData.audioClips = creatureDataTemplate.audioClips;
        newCreatureData.creatureMemberCount = 1;
        newCreatureData.creatureType = CreatureData.CreatureType.Drum;
        newCreatureData.sequenceLengthMultiplier = 1;
        newCreatureData.sequenceLength = 16;

        selectedCreatureData = newCreatureData;

        //implement creatureFamily
        //selectedCreatureData is implemented in Start() in CreatureFamily
        GameObject newCreatureFamilyObject = Instantiate(creatureFamilyPrefab);
        newCreatureFamilyObject.transform.position = _position;
        Debug.Log("Assigning newCreatureFamilyObject: " + newCreatureFamilyObject.name);
        newCreatureFamilyObject.name = "CreatureFamily_" + _model.name;
        CreatureFamily newCreatureFamilyScript = newCreatureFamilyObject.GetComponent<CreatureFamily>();
        newCreatureFamilyScript.Initialize(newCreatureData);
        
        _model.transform.parent = newCreatureFamilyScript.creatureMesh.GetComponentInChildren<CreatureMemberDefault>().transform;
        _model.transform.localPosition = Vector3.zero;

        tempMonaCreatureFamily = newCreatureFamilyObject.GetComponent<CreatureFamily>();
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
