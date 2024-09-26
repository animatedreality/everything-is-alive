using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PersistentStorageManager : MonoBehaviour
{
    //turn this to a singleton
    public static PersistentStorageManager instance;
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

    public Sprite creatureImage;
    public List<CustomCreatureData> customCreatures = new List<CustomCreatureData>();
    string audioClipFilePath = "Sounds/";
    string creatureModelFilePath = "MonaModels/";
    string creatureModelImageFilePath = "MonaModelImages/";
    private string filePath;
    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.persistentDataPath + "/customCreatures.json";
        customCreatures = LoadAllCustomCreatures();
    }


    public void SaveCustomCreature(string creatureName, string audioClipName){
        creatureImage = Resources.Load<Sprite>(creatureModelImageFilePath + creatureName);
        CustomCreatureData customCreatureData = new CustomCreatureData(creatureName, audioClipName, creatureImage);
        List<CustomCreatureData> newCustomCreatures = new List<CustomCreatureData>();
        Debug.Log("Saving custom creature" + creatureName + " with audio clip " + audioClipName);

        // Check if the file exists and read existing data
        if (File.Exists(filePath)) {
            string existingJson = File.ReadAllText(filePath);
            newCustomCreatures = JsonUtility.FromJson<CustomCreatureList>(existingJson).creatures;
        }

        // Check if the creatureName already exists
        bool creatureExists = false;
        for (int i = 0; i < newCustomCreatures.Count; i++) {
            if (newCustomCreatures[i].creatureName == customCreatureData.creatureName) {
                newCustomCreatures[i] = customCreatureData; // Update existing creature
                Debug.Log("Updating existing creature" + customCreatureData.creatureName + " with audio clip " + customCreatureData.audioClipName);
                creatureExists = true;
                break;
            }
        }

        // If the creature does not exist, add it to the list
        if (!creatureExists) {
            newCustomCreatures.Add(customCreatureData);
            Debug.Log("Adding new creature" + customCreatureData.creatureName + " with audio clip " + customCreatureData.audioClipName);
        }

        // Save the updated list back to the file
        CustomCreatureList updatedList = new CustomCreatureList { creatures = newCustomCreatures };
        string json = JsonUtility.ToJson(updatedList);
        File.WriteAllText(filePath, json);

        customCreatures = newCustomCreatures;
    }

    public AudioClip LoadAudioClipFromCreature(string creatureName){
        List<CustomCreatureData> customCreatures = LoadAllCustomCreatures();
        foreach(CustomCreatureData customCreature in customCreatures){
            if(customCreature.creatureName == creatureName){
                return Resources.Load<AudioClip>(audioClipFilePath + customCreature.audioClipName);
            }
        }
        return null;
    }

    public Sprite LoadCreatureImage(string creatureName){
        //find the creature
        List<CustomCreatureData> customCreatures = LoadAllCustomCreatures();
        foreach(CustomCreatureData customCreature in customCreatures){
            if(customCreature.creatureName == creatureName){
                Debug.Log("Loading creature image" + customCreature.creatureName);
                //load custom creature image from /MonaModelImages/
                return Resources.Load<Sprite>(creatureModelImageFilePath + creatureName);
            }
        }
        Debug.LogError("Creature " + creatureName + " image not found");
        return null;
    }

    public List<CustomCreatureData> LoadAllCustomCreatures(){
        List<CustomCreatureData> customCreatures = new List<CustomCreatureData>();
        if(File.Exists(filePath)){
            string existingJson = File.ReadAllText(filePath);
            customCreatures = JsonUtility.FromJson<CustomCreatureList>(existingJson).creatures;
        }
        return customCreatures;
    }

    public List<string> LoadAllCustomCreatureNames(){
        List<string> customCreatureNames = new List<string>();
        List<CustomCreatureData> customCreatures = LoadAllCustomCreatures();
        foreach(CustomCreatureData customCreature in customCreatures){
            customCreatureNames.Add(customCreature.creatureName);
        }
        return customCreatureNames;
    }
}

[System.Serializable]
public class CustomCreatureData{
    public string creatureName;
    public string audioClipName;
    public Sprite creatureImage;
    //instead of storing the gameObject, store a path to generate gameObject prefabs
    public GameObject creatureGameObject;
    public CustomCreatureData(string creatureName, string audioClipName, Sprite creatureImage){
        this.creatureName = creatureName;
        this.audioClipName = audioClipName;
        this.creatureImage = creatureImage;
    }
}

public class CustomCreatureList{
    public List<CustomCreatureData> creatures;
}