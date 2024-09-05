using System.Collections;
using System.Collections.Generic;
using Audio;
using Oculus.Interaction.Samples;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject menuWelcome, menuInGame, menuSelectModel, menuMakeInstrument;

    [Header("Preview Creature")]
    public GameObject previewCreature;//for INGAME, when about to spawn a creature
    public GameObject previewCustomCreature;//for MAKINSTRUMENT, when creating a new creature from scratch
    [Header("Make Instrument")]
    public Transform modelSpawnPoint;
    public GameObject modelPreview;

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
        Debug.Log("Creatures images.Length: " + images.Length);
        foreach (Object image in images)
        {
            GameObject button = Instantiate(PrefabManager.instance.buttonCreaturePrefab, defaultContentContainer);
            button.name = image.name;
            button.GetComponent<UnityEngine.UI.Image>().sprite = (Sprite)image;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCreatureButtonPressed(image.name));
            generatedCreatures.Add(image.name, 0);
        }

        //Load all images from Resrouces/MonaImages and instantiate them as buttons in the monaContentContainer
        Object[] monaCreatureImages = Resources.LoadAll("MonaCreatureImages", typeof(Sprite));
        foreach (Object image in monaCreatureImages)
        {
            GameObject button = Instantiate(PrefabManager.instance.buttonCreaturePrefab, monaContentContainer);
            button.name = image.name;
            button.GetComponent<UnityEngine.UI.Image>().sprite = (Sprite)image;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCreatureButtonPressed(image.name));
            generatedCreatures.Add(image.name, 0);
        }

        //Load all images from Resrouces/MonaModelImages and instantiate them as buttons in the monaModelContentContainer
        Object[] monaModelImages = Resources.LoadAll("MonaModelImages", typeof(Sprite));
        foreach (Object image in monaModelImages)
        {
            GameObject button = Instantiate(PrefabManager.instance.buttonCreaturePrefab, monaModelContentContainer);
            button.name = image.name;
            button.GetComponent<UnityEngine.UI.Image>().sprite = (Sprite)image;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnMonaModelButtonPressed(image.name));
        }

        //Instantiate buttons based on the amount of audio clips in Resources/Sounds
        Object[] audioClips = Resources.LoadAll("Sounds", typeof(AudioClip));
        foreach (Object clip in audioClips)
        {
            GameObject button = Instantiate(PrefabManager.instance.buttonAudioPrefab, audioContentContainer);
            button.GetComponent<AudioSource>().clip = (AudioClip)clip;
            button.name = clip.name;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnAudioButtonPressed(clip.name));
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
    }

    public void SetGameStateWithString(string _gameState){
        SetGameState((GameState)System.Enum.Parse(typeof(GameState), _gameState));
        Debug.Log("SetGameStateWithString" + _gameState);
        Debug.Log("currentGameState" + currentGameState);
    }
    public void ReturnToPreviousGameState(){
        SetGameState(previousGameState);
    }

    public GameState GetGameState(){
        return currentGameState;
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
    
    public void OnMonaModelButtonPressed(string modelName){
        if(modelPreview != null){
            Destroy(modelPreview);
        }
        modelPreview = Instantiate(Resources.Load("MonaModels/" + modelName)) as GameObject;
        modelPreview.name = modelName;
        modelPreview.transform.position = modelSpawnPoint.position;
        modelPreview.transform.localScale = Vector3.one * 0.1f;
        modelPreview.transform.parent = modelSpawnPoint;
        SetGameState(GameState.MAKEINSTRUMENT);
    }

    //create a customCreaturePreview if there is no customCreaturePreview
    //if there is a customCreaturePreview, swap the audioClip from the customCreaturePreview
    public void OnAudioButtonPressed(string audioName){
        Debug.Log("OnAudioButtonPressed" + audioName);
        //spawn a preview
        if(modelPreview == null){
            Debug.Log("No model preview found");
            return;
        }
        if(previewCustomCreature != null){
            previewCustomCreature.GetComponent<CreatureModelSwapper>().UpdateAudioClip(Resources.Load<AudioClip>("Sounds/" + audioName));
        }
        else{
            //turn the model preview into a creature, assign audio clip to it
            previewCustomCreature = Instantiate(PrefabManager.instance.creatureTemplatePrefab);
            Vector3 originalGlobalScale = modelPreview.transform.lossyScale;
            previewCustomCreature.transform.parent = modelSpawnPoint;
            previewCustomCreature.transform.localPosition = Vector3.zero;
            previewCustomCreature.transform.localRotation = Quaternion.identity;
            previewCustomCreature.GetComponent<CreatureModelSwapper>().InitiateCreature(modelPreview, Resources.Load<AudioClip>("Sounds/" + audioName), originalGlobalScale);
        }
    }

    public void SpawnCreaturePreview(){
        if(previewCreature != null){
            Destroy(previewCreature);
        }
        previewCreature = Global.instance.SpawnCreaturePreview(selectedCreatureName, creatureSpawnPoint.position);
        previewCreature.transform.parent = creatureSpawnPoint;
    }

    void DestroyPreviewCreature(){
        if(previewCreature != null){
            Destroy(previewCreature);
            previewCreature = null;
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
            Global.instance.SpawnCreature(selectedCreatureName, creatureSpawnPoint.position);


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
