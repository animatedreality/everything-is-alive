using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Meta.XR.Util;
public class UIManager : MonoBehaviour
{
    public static UIManager i;

    public GameObject mainMenu, mainMenuToggle, hintToggle;
    public GameObject[] GameplayHints;
    public GameObject defaultCreatureContainer;
    public GameObject audioClipsContainer, audioScrollviewContainer;
    public UIButtonContainer defaultCreatureUIButtonContainer;
    public Color buttonSelectedColor, buttonUnselectedColor;

    [Header("Mona")]
    public bool isMonaLoggedIn = false;
    public GameObject monaLoginScreen, monaModal;
    public OVRVirtualKeyboard virtualKeyboard;

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
        InitializeAudioClipsContainer();
        SetMonaLoginScreens();
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

    void InitializeAudioClipsContainer(){
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
        audioScrollviewContainer.SetActive(isMonaLoggedIn);
        monaLoginScreen.SetActive(!isMonaLoggedIn);
    }

    public void UpdateKeyboardPosition(){
        virtualKeyboard.gameObject.SetActive(true);
        virtualKeyboard.UseSuggestedLocation(OVRVirtualKeyboard.KeyboardPosition.Custom);
        Vector3 offset = new Vector3(0, -1.2f, 0);
        virtualKeyboard.transform.position = monaModal.transform.position + offset;
    }


}
