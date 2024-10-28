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

    [Header("Mona related")]
    public GameObject tempMona3DModel;
    public Vector3 tempMona3DModelScale;
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
            if(selectedCreatureData.name.Contains("MonaModel")){
                //add the 3D model from persistent storage to creatureData and then initiate CreatureFamily
                selectedCreatureData.prefab = H_PersistentStorage.LoadGLB(selectedCreatureData.name);
                return;
            }

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
        //if globalPlay is false, start the game
        if(!AudioManager.i.globalPlay){
            AudioManager.i.Play();
        }
        //implement creatureData
        CreatureData monaCreatureData = ScriptableObject.CreateInstance<CreatureData>();
        monaCreatureData.name = _model.name;
        monaCreatureData.sprite = creatureDataTemplate.sprite;//set this from Mona later
        monaCreatureData.audioClips = creatureDataTemplate.audioClips;
        monaCreatureData.creatureMemberCount = 1;
        monaCreatureData.creatureType = CreatureData.CreatureType.Drum;
        monaCreatureData.sequenceLengthMultiplier = 1;
        monaCreatureData.sequenceLength = 16;

        selectedCreatureData = monaCreatureData;

        //implement creatureFamily
        //selectedCreatureData is implemented in Start() in CreatureFamily
        GameObject monaCreatureFamilyObj = Instantiate(creatureFamilyPrefab);
        monaCreatureFamilyObj.transform.position = _position;
        Debug.Log("Assigning monaCreatureFamilyObj: " + monaCreatureFamilyObj.name);
        monaCreatureFamilyObj.name = "CreatureFamily_" + _model.name;
        CreatureFamily monaCreatureFamily = monaCreatureFamilyObj.GetComponent<CreatureFamily>();
        monaCreatureFamily.Initialize(monaCreatureData);
        
        _model.transform.parent = monaCreatureFamily.creatureMesh.GetComponentInChildren<CreatureMemberDefault>().transform;
        _model.transform.localPosition = Vector3.zero;

        tempMona3DModel = _model;
        tempMona3DModelScale = _model.transform.localScale;
        tempMonaCreatureFamily = monaCreatureFamilyObj.GetComponent<CreatureFamily>();
        //tempMonaCreatureFamily is automatically initialized with selectedCreatureData


    }

    //call this with a custom button when Mona object pops up and is selected
    public void SaveTempMonaCreature(){
        //save tempMonaCreatureData to persistent storage
        if(selectedCreatureData != tempMonaCreatureFamily.creatureData){
            Debug.LogError("SaveTempMonaCreature: selectedCreatureData is not the same as tempMonaCreatureFamily.creatureData");
            return;
        }
        Debug.Log("Saving tempMonaCreatureData: " + selectedCreatureData.name);
        H_PersistentStorage.SaveCreatureData(selectedCreatureData);

        //save the 3D model as a GLB file
        //save the 3D model to persistent storage, as well as its local scale
        H_PersistentStorage.SaveGLB(tempMona3DModel, tempMona3DModelScale, tempMona3DModel.name);

        //add a button to the main menu
        UIManager.i.AddNewCreatureButton(selectedCreatureData);
        
       //clear tempMonaCreatureFamily and selectedCreatureData
       tempMonaCreatureFamily = null;
       selectedCreatureData = null;
    }

    public void DeleteTempMonaCreature(){
        if(selectedCreatureData == tempMonaCreatureFamily.creatureData){
            selectedCreatureData = null;
        }
        Destroy(tempMonaCreatureFamily);
    }
}
