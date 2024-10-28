using System.Collections;
using System.Collections.Generic;
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
    public GameObject buttonPrefab, audioBttnPrefab;
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
        defaultCreatureUIButtonContainer.Initialize(_creatureDataList);
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
            //button.GetComponent<Button>().onClick.AddListener(() => OnAudioClipButtonPressed((AudioClip)clip));
        }
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


}
