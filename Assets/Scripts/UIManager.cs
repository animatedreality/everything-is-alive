using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Meta.XR.Util;
using Monaverse.Examples;
public class UIManager : MonoBehaviour
{
    public static UIManager i;

    public GameObject mainMenu, mainMenuToggle, hintToggle;
    public GameObject[] GameplayHints;
    public GameObject defaultCreatureContainer;
    public GameObject audioClipsContainer, audioClipsMenu;
    public UIButtonContainer defaultCreatureUIButtonContainer;
    public Color buttonSelectedColor, buttonUnselectedColor;

    [Header("Mona")]
    public bool isMonaLoggedIn = false;
    public GameObject monaLoginScreen, monaObject, monaModel3DContainer, virtualKeyboard;
    public MonaManager_Nes customMonaManager;

    [Header("Prefabs")]
    public GameObject buttonPrefab;
    public GameObject audioBttnPrefab;
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
    // Start is called before the first frame update
    public void Initialize()
    {
        //change this later based on game state
        mainMenu.SetActive(true);
        audioClipsMenu.SetActive(false);
        //monaObject.SetActive(false);
        virtualKeyboard.SetActive(false);
        InitializeCreatureContainer(defaultCreatureContainer, CreatureManager.i.creatureDataList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializeCreatureContainer(GameObject _container, List<CreatureData> _creatureDataList){
        if(!_container.GetComponent<UIButtonContainer>())
            _container.AddComponent<UIButtonContainer>();

        defaultCreatureUIButtonContainer = _container.GetComponent<UIButtonContainer>();
        //initialize all the buttons
        foreach(CreatureData creatureData in _creatureDataList){
            GameObject button = Instantiate(UIManager.i.buttonPrefab, defaultCreatureUIButtonContainer.transform);
            button.GetComponent<UIButton>().Initialize(creatureData, defaultCreatureUIButtonContainer);
        }

        //load all creatureData from persistent storage
        LoadAllSavedCreatureButtons();
    }

    private void LoadAllSavedCreatureButtons()
    {
        // Get all json files in persistent data path that end with _CreatureData.json
        string[] creatureFiles = Directory.GetFiles(Application.persistentDataPath, "*_CreatureData.json");
        
        foreach (string filePath in creatureFiles)
        {
            // Extract creature name from filename (remove _CreatureData.json)
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string creatureName = fileName.Replace("_CreatureData", "");
            
            // Load creature data using existing MonaManager function
            CreatureData creatureData = CreatureManager.i.monaManager_Nes.LoadCreatureData(creatureName);
            
            if (creatureData != null)
            {
                // Add button for this creature
                AddNewCreatureButton(creatureData);
            }
        }
    }

    public void InitializeAudioClipsContainer(){
        audioClipsMenu.SetActive(true);
        //load all audio clips from Resources/AudioClips
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");
        AudioManager.i.audioClips = new List<AudioClip>(clips);
        foreach(AudioClip clip in clips){
            GameObject button = Instantiate(audioBttnPrefab, audioClipsContainer.transform);
            if(button.GetComponentInChildren<TextMeshProUGUI>()){
                button.GetComponentInChildren<TextMeshProUGUI>().text = clip.name;
                button.name = clip.name;
            }
            //set this to connect audio to new object later on
            button.GetComponent<Button>().onClick.AddListener(() => OnAudioClipButtonPressed((AudioClip)clip));
        }
    }

    void OnAudioClipButtonPressed(AudioClip _clip){
        Debug.Log("Audio Clip Button Pressed: " + _clip.name);
        //play the audioClip
        AudioManager.i.PlayAudioClip(_clip);
        AudioManager.i.SwapAudioClip(_clip, CreatureManager.i.tempMonaCreatureFamily);
    }

    public void ToggleMainMenu(){
        mainMenu.SetActive(!mainMenu.activeSelf);
        mainMenuToggle.GetComponent<H_Selection>().ToggleSelected();
    }

    public void ToggleGameplayHint(){
        hintToggle.GetComponent<H_Selection>().ToggleSelected();
        foreach(GameObject hint in GameplayHints){
            hint.SetActive(!hint.activeSelf);
        }
    }

    public void SetMonaLoginScreens(){

        //isMonaLoggedIn is a public variable that is set by MonaManager
        monaLoginScreen.SetActive(true);
        monaObject.SetActive(true);
        virtualKeyboard.transform.position = monaObject.transform.position + new Vector3(0, -0.25f, 0);
        //virtualKeyboard.transform.rotation = monaObject.transform.rotation;
        virtualKeyboard.SetActive(true);
        monaModel3DContainer.transform.position = monaObject.transform.position;
        customMonaManager.StartMonaModel();
    }

    public void AddNewCreatureButton(CreatureData _creatureData){
        GameObject button = Instantiate(buttonPrefab, defaultCreatureUIButtonContainer.transform);
        button.GetComponent<UIButton>().Initialize(_creatureData, defaultCreatureUIButtonContainer);
    }


}
