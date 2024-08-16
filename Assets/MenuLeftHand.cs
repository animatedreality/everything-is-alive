using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLeftHand : MonoBehaviour
{
    public RectTransform scrollviewContent;
    public static MenuLeftHand instance;

    public GameObject canvasGameObject;

    string selectedCreatureName = "";

    public Transform creatureSpawnPoint;

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
        Debug.Log("MenuLeftHand Start");
        //load all images from Resources/CreatureImages and instantiate them as buttons in the scrollviewContent
        Object[] images = Resources.LoadAll("CreatureImages", typeof(Sprite));
        Debug.Log("images.Length: " + images.Length);
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
        bool rightControllerBButton = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);
        bool leftControllerXButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);

        if (rightControllerBButton)
        {
            try
            {
                GameObject creature = Instantiate(Resources.Load("Creatures/" + selectedCreatureName)) as GameObject;
                creature.transform.position = creatureSpawnPoint.position;
            }
            catch (System.Exception e)
            {
                Debug.Log("Error instantiating creature: " + e.Message);
            }
        }
        if (leftControllerXButton)
        {
            canvasGameObject.SetActive(!canvasGameObject.activeSelf);
        }

    }
}
