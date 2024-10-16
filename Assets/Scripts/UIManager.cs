using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager i;

    public GameObject mainMenu, toggleMainMenuIndicator;
    public GameObject defaultCreatureContainer;
    public GameObject monaCreatureContainer;
    public GameObject audioClipsContainer;
    public UIButtonContainer defaultCreatureUIButtonContainer, monaCreatureUIButtonContainer;
    public Color buttonSelectedColor, buttonUnselectedColor;

    [Header("Mona")]
    public bool isMonaLoggedIn = false;
    public GameObject monaLoginScreen;

    [Header("Prefabs")]
    public GameObject buttonPrefab;
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
        //creature tab is selected by default
        SelectCreatureTab();

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

    public void ToggleMainMenu(){
        mainMenu.SetActive(!mainMenu.activeSelf);
        toggleMainMenuIndicator.GetComponent<H_Selection>().ToggleSelected();
    }

    public void SelectCreatureTab(){
        defaultCreatureContainer.SetActive(true);
        audioClipsContainer.SetActive(false);
        monaCreatureContainer.SetActive(false);
        monaLoginScreen.SetActive(false);
    }

    public void SelectNewTab(){
        //if already logged into Mona, show Mona's Modal + Audio Clips
        //if not logged into Mona, show login button + Mona Login Modal
        if(isMonaLoggedIn){
            audioClipsContainer.SetActive(true);
        }
        else{
            monaLoginScreen.SetActive(true);
        }
        defaultCreatureContainer.SetActive(false);
    }


}
