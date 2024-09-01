using System.Collections;
using System.Collections.Generic;
using Audio;
using Oculus.Interaction.Samples;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public RectTransform scrollviewContent;
    public static MenuManager instance;

    public GameObject canvasGameObject;

    string selectedCreatureName = "";

    public Transform creatureSpawnPoint;

    //Buttons
    bool rightControllerBButton, leftControllerXButton, rightTriggerPress, rightGripPress;

    //Volume
    public GameObject volumeUI;
    float rightJoystickVertical;
    public float currentCreatureVolume;
    bool changeVolumeStart;
    public Slider volSlider;

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
        //load all images from Resources/CreatureImages and instantiate them as buttons in the scrollviewContent
        Object[] images = Resources.LoadAll("CreatureImages", typeof(Sprite));
        Debug.Log("Creatures images.Length: " + images.Length);
        foreach (Object image in images)
        {
            GameObject button = Instantiate(PrefabManager.instance.buttonCreaturePrefab, scrollviewContent);
            button.name = image.name;
            button.GetComponent<UnityEngine.UI.Image>().sprite = (Sprite)image;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCreatureButtonPressed(image.name));
        }
        OnCreatureButtonPressed(images[0].name);
        //button dimensions are 64x64, so set the scrollviewContent height to the number of buttons times 64
        scrollviewContent.sizeDelta = new Vector2(scrollviewContent.sizeDelta.x, images.Length * 64);
        canvasGameObject.SetActive(false);
        volSlider = volumeUI.GetComponentInChildren<Slider>();
        volumeUI.SetActive(false);
    }

    public void OnCreatureButtonPressed(string creatureName)
    {
        selectedCreatureName = creatureName;
        //highlight the selected button and un-highlight the others
        foreach (Transform child in scrollviewContent)
        {
            if (child.name == creatureName)
            {
                child.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                child.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    void Update()
    {
        rightControllerBButton = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);
        leftControllerXButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);
        rightTriggerPress = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        rightGripPress = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
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

        if (rightControllerBButton)
        {
            try
            {
                Global.instance.SpawnCreature(selectedCreatureName, creatureSpawnPoint.position);
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
                canvasGameObject.SetActive(!canvasGameObject.activeSelf);
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

    }

    public void ToggleVolumeUI(bool _toggle, float _volume){
        volumeUI.SetActive(_toggle);
        if(_toggle){
            currentCreatureVolume = _volume;
        }
        changeVolumeStart = _toggle;
    }
}
