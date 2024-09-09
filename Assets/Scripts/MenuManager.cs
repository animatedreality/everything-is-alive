using System.Collections;
using System.Collections.Generic;
using Audio;
using Oculus.Interaction.Samples;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState{
    WELCOME,
    INGAME,
    SELECTMODEL,
    MAKEINSTRUMENT
}
public class MenuManager : MonoBehaviour
{
    public RectTransform defaultContentContainer, monaContentContainer, monaModelContentContainer, audioContentContainer;
    public static MenuManager instance;

    public GameObject leftMenuContainer;

    public string selectedCreatureName = "";

    public Transform creatureSpawnPoint;

    //Buttons
    bool rightControllerBButton, leftControllerXButton, rightTriggerPress, rightGripPress, leftMenuPress, leftJoystickLeft;
    bool previousLeftJoystickLeft;
    //Volume
    public GameObject volumeUI;
    float rightJoystickVertical;
    public float currentCreatureVolume;
    bool changeVolumeStart;
    public Slider volSlider;
    [SerializeField]
    public Dictionary<string, int> generatedCreatures = new Dictionary<string, int>();
    int maxGeneratedAmount = 3;

    [Header("Game State")]
    public GameState currentGameState = GameState.INGAME;
    private GameState previousGameState = GameState.INGAME;
    public GameObject menuWelcome, menuInGame, menuSelectModel, menuMakeInstrument, welcomeGameObject;

    [Header("Preview Creature")]
    public GameObject previewCreature;//for INGAME, when about to spawn a creature
    public GameObject previewCustomCreature;//for MAKINSTRUMENT, when creating a new creature from scratch
    [Header("Tracking Creature Lists")]
    public List<string> customCreatureNames = new List<string>();
    public List<string> defaultCreatureNames = new List<string>();//used to check which creatures are default and which are spawned from Mona?
    [Header("Make Instrument")]
    public Transform modelSpawnPoint;
    public GameObject saveCreatureButton;
    public string previewCustomCreatureAudioName;

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

    void Start()
    {
        SetGameState(currentGameState);
        previousGameState = currentGameState;

        //load all images from Resources/CreatureImages and instantiate them as buttons in the defaultContentContainer
        Object[] images = Resources.LoadAll("CreatureImages", typeof(Sprite));
        //Debug.Log("Creatures images.Length: " + images.Length);
        foreach (Object image in images)
        {
            GameObject button = Instantiate(PrefabManager.instance.buttonCreaturePrefab, defaultContentContainer);
            button.name = image.name;
            button.GetComponent<UnityEngine.UI.Image>().sprite = (Sprite)image;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCreatureButtonPressed(image.name));
            generatedCreatures.Add(image.name, 0);
            defaultCreatureNames.Add(image.name);
        }

        //Load all images from Resrouces/MonaModelImages and instantiate them as buttons in the monaModelContentContainer
        //check from customCreatureNames
        customCreatureNames = PersistentStorageManager.instance.LoadAllCustomCreatureNames();
        //if monaModelImages names contains customCreatureNames, do not instantiate this button
        Object[] monaModelImages = Resources.LoadAll("MonaModelImages", typeof(Sprite));
        foreach (Object image in monaModelImages)
        {
            //if the image is NOT a CustomCreature yet
            if(!customCreatureNames.Contains(image.name)){
                GameObject button = Instantiate(PrefabManager.instance.buttonCreaturePrefab, monaModelContentContainer);
                button.name = image.name;
                button.GetComponent<UnityEngine.UI.Image>().sprite = (Sprite)image;
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnMonaModelButtonPressed(image.name));
            }else{
                //if the image is a Creature, instantiate it in the monaContentContainer
                AddCustomCreatureButton(monaContentContainer, image.name);
            }
        }

        //Instantiate buttons based on the amount of audio clips in Resources/Sounds
        Object[] audioClips = Resources.LoadAll("Sounds", typeof(AudioClip));
        foreach (Object clip in audioClips)
        {
            GameObject button = Instantiate(PrefabManager.instance.buttonAudioPrefab, audioContentContainer);
            button.GetComponent<AudioSource>().clip = (AudioClip)clip;
            button.name = clip.name;
            if(button.GetComponentInChildren<TextMeshProUGUI>()){
                button.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(true);
                button.GetComponentInChildren<TextMeshProUGUI>().text = clip.name;
            }
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnAudioButtonPressed((AudioClip)clip));
        }

        //select the first button by default
        //OnCreatureButtonPressed(images[0].name);

        //button dimensions are 64x64, so set the defaultContentContainer height to the number of buttons times 64
        //defaultContentContainer.sizeDelta = new Vector2(defaultContentContainer.sizeDelta.x, images.Length * 64);
        //leftMenuContainer.SetActive(true);
        volSlider = volumeUI.GetComponentInChildren<Slider>();
        volumeUI.SetActive(false);
    }

    public void SetGameState(GameState _gameState){
        previousGameState = currentGameState;
        currentGameState = _gameState;
        menuWelcome.SetActive(currentGameState == GameState.WELCOME);
        menuInGame.SetActive(currentGameState == GameState.INGAME);
        menuSelectModel.SetActive(currentGameState == GameState.SELECTMODEL);
        menuMakeInstrument.SetActive(currentGameState == GameState.MAKEINSTRUMENT);
        saveCreatureButton.SetActive(false);
        if(_gameState != GameState.INGAME){
            DestroyPreviewCreature();
            Global.instance.DeselectCurrentCreature();
        }
        welcomeGameObject.SetActive(_gameState == GameState.WELCOME);
        //backButton.SetActive(_gameState == GameState.MAKEINSTRUMENT || _gameState == GameState.SELECTMODEL);
    }

    public void SetGameStateWithString(string _gameState){
        SetGameState((GameState)System.Enum.Parse(typeof(GameState), _gameState));
        Debug.Log("SetGameStateWithString" + _gameState);
        Debug.Log("currentGameState" + currentGameState);
    }
    public void ReturnToPreviousGameState(){
        if(currentGameState == GameState.SELECTMODEL)
        {
            DeselectButtons(monaModelContentContainer);
            selectedCreatureName = "";
            SetGameState(GameState.INGAME);
        }
        if(currentGameState == GameState.MAKEINSTRUMENT)
        {
            DestroyPreviewCustomCreature();
            previewCustomCreatureAudioName = "";
            saveCreatureButton.SetActive(false);
            SetGameState(GameState.SELECTMODEL);
        }
    }

    public GameState GetGameState(){
        return currentGameState;
    }

    void DeselectButtons(Transform container){
        foreach(Transform child in container){
            if(child.GetComponent<Button>()){
            child.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    public void OnCreatureButtonPressed(string creatureName)
    {
        Debug.Log("OnCreatureButtonPressed" + creatureName);
        //Generate creature from here
        selectedCreatureName = creatureName;
        //spawn a preview
        SpawnCreaturePreview();

        //highlight the selected button and un-highlight the others
        foreach (Transform child in defaultContentContainer)
        {
            child.GetComponent<UnityEngine.UI.Image>().color = (child.name == creatureName) ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
        }
        foreach(Transform child in monaContentContainer){
            child.GetComponent<UnityEngine.UI.Image>().color = (child.name == creatureName) ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
        }
    }
    
    //Create a creature
    public void OnMonaModelButtonPressed(string modelName){
        DestroyPreviewCustomCreature();
        //here instantiate a new Creature with MonaModel as the visual
        previewCustomCreature = SpawnCustomCreatureInPreview(modelSpawnPoint, modelName);
        previewCustomCreature.GetComponent<CreatureModelSwapper>().UpdateAudioClip(Resources.Load<AudioClip>("Sounds/" + "crash-01"));
        previewCustomCreatureAudioName = "";//set audioname to empty
        SetGameState(GameState.MAKEINSTRUMENT);
    }

    //create a customCreaturePreview if there is no customCreaturePreview
    //if there is a customCreaturePreview, swap the audioClip from the customCreaturePreview
    public void OnAudioButtonPressed(AudioClip clip){
        Debug.Log("OnAudioButtonPressed" + clip.name);
        //spawn a preview
        if(previewCustomCreature == null){
            Debug.Log("No model preview found");
            return;
        }
        //previewCustomCreature is the container that Mona Model lives in
        previewCustomCreature.GetComponent<CreatureModelSwapper>().UpdateAudioClip(clip);
        previewCustomCreatureAudioName = clip.name;
        saveCreatureButton.SetActive(true);
    }

    GameObject SpawnCustomCreatureInPreview(Transform parent, string modelName){
        GameObject creaturePreview = SpawnCustomCreature(parent.position, modelName);
        creaturePreview.transform.parent = parent;
        return creaturePreview;
    }

    GameObject SpawnCustomCreature(Vector3 _spawnPoint, string _modelName)
    {
        GameObject modelPreview = Instantiate(Resources.Load("MonaModels/" + _modelName)) as GameObject;
        GameObject newCreature = Instantiate(PrefabManager.instance.creatureTemplatePrefab);
        modelPreview.transform.parent = newCreature.transform;
        modelPreview.transform.localPosition = Vector3.zero;
        //add 180 on y axis to localRotation
        modelPreview.transform.localRotation = Quaternion.Euler(0, 180, 0);
        modelPreview.transform.localScale = Vector3.one * 0.05f;
        Debug.Log("modelPreview.transform.localScale" + modelPreview.transform.localScale);
        newCreature.name = _modelName;
        newCreature.transform.position = _spawnPoint;
        newCreature.transform.localRotation = Quaternion.identity;
        //here we need to get the AudioClip name from PersistentStorageManager
        AudioClip audioClip = PersistentStorageManager.instance.LoadAudioClipFromCreature(_modelName);
        newCreature.GetComponent<CreatureModelSwapper>().InitiateCreature(modelPreview, audioClip);
        return newCreature;
    }

    //after choosing audio clip, save the creature, remove it
    public void OnSaveCreatureButtonPressed(){
        if (previewCustomCreature == null || previewCustomCreatureAudioName == "")
        {
            Debug.Log("No custom creature found, previewCustomCreature is null");
            return;
        }
        //Only saves the creature after an audio is selected
        Debug.Log("OnSaveCreatureButtonPressed");

        //remove this creature's button from monaModelContentContainer
        foreach(Transform child in monaModelContentContainer){
           if(child.name == previewCustomCreature.name){
               Destroy(child.gameObject);
               break;
           }
        }

        //add this creature to the creatureGameObject component  customCreatures list in PersistentStorageManager
        PersistentStorageManager.instance.SaveCustomCreature(previewCustomCreature.name, previewCustomCreatureAudioName);


        //add this creature's button to monaContentContainer
        //Create Custom Creature Button
        AddCustomCreatureButton(monaContentContainer, previewCustomCreature.name);

        Destroy(previewCustomCreature);
        previewCustomCreature = null;

        SetGameState(GameState.INGAME);
    }

    void AddCustomCreatureButton(Transform container, string creatureName){
        Debug.Log("AddCustomCreatureButton" + creatureName);
        GameObject button = Instantiate(PrefabManager.instance.buttonCreaturePrefab, container);
        button.name = creatureName;
        Sprite creatureImage = PersistentStorageManager.instance.LoadCreatureImage(creatureName);
        Debug.Log("check if Sprite is loaded" + creatureImage.name + " " + creatureName);
        button.GetComponent<UnityEngine.UI.Image>().sprite = creatureImage;
        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCreatureButtonPressed(creatureName));
        generatedCreatures.Add(creatureName, 0);
        if(!customCreatureNames.Contains(creatureName)){
            customCreatureNames.Add(creatureName);
        }
    }

    public void SpawnCreaturePreview(){
        if(previewCreature != null){
            Destroy(previewCreature);
        }
        previewCreature = Global.instance.SpawnCreaturePreview(selectedCreatureName, creatureSpawnPoint.position);
        if(previewCreature == null){
            Debug.Log("Failed to spawn creature preview");
            return;
        }
        previewCreature.transform.parent = creatureSpawnPoint;
    }

    void DestroyPreviewCreature(){
        if(previewCreature != null){
            Destroy(previewCreature);
            previewCreature = null;
        }
    }

    void DestroyPreviewCustomCreature(){
        if(previewCustomCreature != null){
            Destroy(previewCustomCreature);
            previewCustomCreature = null;
        }
    }

    public void SpawnCreature(){
        Debug.Log("SpawnCreature" + selectedCreatureName);
        if(selectedCreatureName == ""){
            Debug.Log("No creature selected");
            return;
        }
        if(generatedCreatures[selectedCreatureName] < maxGeneratedAmount){
            Debug.Log("Creature Name is " + selectedCreatureName);
            if(generatedCreatures.ContainsKey(selectedCreatureName)){
                generatedCreatures[selectedCreatureName]++;
                Debug.Log("Generated Creatures Number" + generatedCreatures[selectedCreatureName]);
            }

            //Destroy preview and create the actual creature
            DestroyPreviewCreature();
            //HERE, if creature were from MONA, spawn it from PersistentStorageManager and custom prefab list
            //add a list of GameObject prefabs to PersistenStorageManager's CustomCreatureData class
            //find it from preview earlier, it should already have been made, just store it before destroying it
            if(defaultCreatureNames.Contains(selectedCreatureName)){
                Global.instance.SpawnCreature(selectedCreatureName, creatureSpawnPoint.position);
            }else if(customCreatureNames.Contains(selectedCreatureName)){
                //spawn from PersistentStorageManager
                SpawnCustomCreature(creatureSpawnPoint.position, selectedCreatureName);
            }


            if(generatedCreatures[selectedCreatureName] == maxGeneratedAmount){
                foreach (Transform child in defaultContentContainer)
                {
                    if (child.name == selectedCreatureName)
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }
                foreach(Transform child in monaContentContainer){
                    if (child.name == selectedCreatureName)
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }
            }
        }
    }

    void Update()
    {
        rightControllerBButton = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);
        leftControllerXButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);
        rightTriggerPress = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        rightGripPress = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        leftMenuPress = OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch);
        leftJoystickLeft = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch).x < -0.5f;

        if(changeVolumeStart){
            rightJoystickVertical = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).y;
            currentCreatureVolume += rightJoystickVertical * Time.deltaTime;
            currentCreatureVolume = Mathf.Clamp01(currentCreatureVolume);
            if(volSlider != null){
                Debug.Log("change vol slider" + currentCreatureVolume);
                volSlider.value = currentCreatureVolume;
            }
            if(Global.instance.currentSelectedCreature)
                Global.instance.currentSelectedCreature.SetVolume(currentCreatureVolume);
            
        }

        //Return to previous game state if left joystick is pressed left
        if(leftJoystickLeft && !previousLeftJoystickLeft){
            Debug.Log("leftJoystickLeft");
            if(previousGameState != currentGameState){
                Debug.Log("ReturnToPreviousGameState");
                ReturnToPreviousGameState();
            }
        }
        previousLeftJoystickLeft = leftJoystickLeft;

        if (rightControllerBButton)
        {
            try
            {
                SpawnCreature();
                Tutorial.instance.DisableRightInstantiateHint();
            }
            catch (System.Exception e)
            {
                Debug.Log("Error instantiating creature: " + e.Message);
            }
        }
        if (leftControllerXButton)
        {
            try{    
                leftMenuContainer.SetActive(!leftMenuContainer.activeSelf);
                Tutorial.instance.DisableLeftMenuHint();
            }
            catch (System.Exception e)
            {
                Debug.Log("Error showing menu: " + e.Message);
            }
        }
        if(rightTriggerPress){
            try{
                Tutorial.instance.DisableRightSelectHint();
            }
            catch (System.Exception e)
            {
                Debug.Log("Error showing menu: " + e.Message);
            }
        }
        if(rightGripPress){
            try{
                Tutorial.instance.DisableRightMoveHint();
            }
            catch (System.Exception e)
            {
                Debug.Log("Error showing menu: " + e.Message);
            }
        }
        if(leftMenuPress){
            try{
                Debug.Log("Left Menu Pressed!!");
                Tutorial.instance.ToggleShowTutorial();
            }
            catch (System.Exception e)
            {
                Debug.Log("Error showing menu: " + e.Message);
            }
        }


    }

    public void ToggleVolumeUI(bool _toggle, float _volume){
        Debug.Log("MenuManagerToggleVolumeUI" + _toggle);
        volumeUI.SetActive(_toggle);
        if(_toggle){
            currentCreatureVolume = _volume;
        }
        changeVolumeStart = _toggle;
    }
}
