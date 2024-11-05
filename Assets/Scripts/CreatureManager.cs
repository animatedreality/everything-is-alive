using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Monaverse.Examples;
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
    public MonaManager_Nes monaManager_Nes;
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

    public async Task SpawnCreature(){
        //if globalPlay is false, start the game
        if(!AudioManager.i.globalPlay){
            AudioManager.i.Play();
        }
        GameObject newCreature = null;
        //Spawn Creature if there has been one selected
        if(selectedCreatureData != null && isInGame){

            //check if the selectedCreatureData is a Mona Model
            Debug.Log("SpawnCreature: selectedCreatureData: " + selectedCreatureData.name);
            if(selectedCreatureData.name.Contains("MonaModel")){
                newCreature = await CreateNewCreature(selectedCreatureData);
                //creatureFamily is initialized in CreateNewCreature
            }
            else{
                newCreature = Instantiate(creatureFamilyPrefab);
                //initialize the creatureFamily
                newCreature.GetComponent<CreatureFamily>().Initialize(selectedCreatureData);
            }

            
            newCreature.transform.position = creatureSpawnPoint.position;
            Debug.Log("SpawnCreature");
        }
    }

    //Creating Creatures from Mona Models
    public async Task<GameObject> CreateNewCreature(CreatureData _creatureData){
        selectedCreatureData = _creatureData;

        GameObject newCreatureModel = await monaManager_Nes.Load3DModelPersistently(_creatureData.name);

        //step1 instantiate creatureFamilyPrefab
        GameObject newCreatureFamily = Instantiate(creatureFamilyPrefab);
        newCreatureFamily.GetComponent<CreatureFamily>().Initialize(_creatureData);
        
        newCreatureFamily.name = _creatureData.name + "_Loaded_Family";
        //step2 swap creatureMesh with 3D model saved with H_PersistentStorage with the creatureData's name
        //GameObject newCreatureModel = await H_PersistentStorage.LoadNewCreatureModel(_creatureData.name);
        
        Debug.Log("Set New Creautre Parent" + newCreatureFamily.name);
        Debug.Log("the new CreatureModel is: " + newCreatureModel.name);
        newCreatureModel.name += "_Loaded_Model";
        newCreatureModel.transform.parent = newCreatureFamily.GetComponent<CreatureFamily>().creatureMesh.GetComponentInChildren<CreatureMemberDefault>().transform;

        monaManager_Nes.ResizeModelToFit(newCreatureModel);
        //custom offsets
        newCreatureModel.transform.localPosition = new Vector3(0, 0.04f, 0);
        newCreatureModel.transform.localScale *= 0.333f;
        newCreatureModel.transform.localRotation *= Quaternion.Euler(0, 0, 0);
        // Vector3 newModelGlobalScale = monaManager_Nes.LoadCreatureScale(_creatureData.name);
        // SetLocalScaleToMatchGlobalScale(newCreatureModel.transform, newModelGlobalScale);
        return newCreatureFamily;
    }

    public void SelectCreatureFamily(CreatureFamily _creatureFamily){
        if(selectedCreatureFamily != null && selectedCreatureFamily != _creatureFamily){
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
        //Debug.Log("Assigning monaCreatureFamilyObj: " + monaCreatureFamilyObj.name);
        monaCreatureFamilyObj.name = "CreatureFamily_" + _model.name;
        CreatureFamily monaCreatureFamily = monaCreatureFamilyObj.GetComponent<CreatureFamily>();
        monaCreatureFamily.Initialize(monaCreatureData);
        
        
        _model.transform.parent = monaCreatureFamily.creatureMesh.GetComponentInChildren<CreatureMemberDefault>().transform;
        _model.transform.localPosition = Vector3.zero;
        _model.transform.localRotation = Quaternion.Euler(0, 180, 0);

        tempMona3DModel = _model;
        tempMona3DModelScale = _model.transform.lossyScale;
        tempMonaCreatureFamily = monaCreatureFamilyObj.GetComponent<CreatureFamily>();

        monaCreatureFamily.meshContainer.transform.localPosition = new Vector3(0, 0.02f, 0);
        //tempMonaCreatureFamily is automatically initialized with selectedCreatureData


    }

    //call this with a custom button when Mona object pops up and is selected
    public void SaveTempMonaCreature(){
        //save tempMonaCreatureData to persistent storage
        if(selectedCreatureData != tempMonaCreatureFamily.creatureData){
            Debug.LogError("SaveTempMonaCreature: selectedCreatureData is not the same as tempMonaCreatureFamily.creatureData");
            return;
        }
        Debug.Log("STEP1 Saving tempMonaCreatureData: " + selectedCreatureData.name);


        monaManager_Nes.SaveNewCreature(selectedCreatureData, tempMona3DModel, tempMona3DModelScale);
        //H_PersistentStorage.SaveNewCreature(selectedCreatureData, tempMona3DModel, tempMona3DModelScale);
        //add a button to the main menu
        UIManager.i.AddNewCreatureButton(selectedCreatureData);
        
       //clear tempMonaCreatureFamily and selectedCreatureData
       //delete the tempMona3DModel
       Destroy(tempMonaCreatureFamily.gameObject);
       tempMonaCreatureFamily = null;
       selectedCreatureData = null;
    }

    public void DeleteTempMonaCreature(){
        if(selectedCreatureData == tempMonaCreatureFamily.creatureData){
            selectedCreatureData = null;
        }
        Destroy(tempMonaCreatureFamily);
    }


    void SetLocalScaleToMatchGlobalScale(Transform transform, Vector3 targetGlobalScale)
    {
        Transform parent = transform.parent;
        Vector3 newLocalScale = targetGlobalScale;

        // Divide the target global scale by each parent's global scale to get the necessary local scale
        while (parent != null)
        {
            newLocalScale.x /= parent.lossyScale.x;
            newLocalScale.y /= parent.lossyScale.y;
            newLocalScale.z /= parent.lossyScale.z;
            parent = parent.parent;
        }

        transform.localScale = newLocalScale;
    }
}
