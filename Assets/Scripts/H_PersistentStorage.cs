using System.IO;
using UnityEngine;

public static class H_PersistentStorage
{
    private static readonly string creatureDataPath = Path.Combine(Application.persistentDataPath, "CreatureData");
    private static readonly string glbDataPath = Path.Combine(Application.persistentDataPath, "GLBFiles");

    static H_PersistentStorage()
    {
        if (!Directory.Exists(creatureDataPath))
            Directory.CreateDirectory(creatureDataPath);

        if (!Directory.Exists(glbDataPath))
            Directory.CreateDirectory(glbDataPath);
    }

    public static void SaveCreatureData(CreatureData creatureData)
    {
        string json = JsonUtility.ToJson(creatureData);
        string filePath = Path.Combine(creatureDataPath, creatureData.name + ".json");
        File.WriteAllText(filePath, json);
        Debug.Log($"Saved CreatureData to {filePath}");
    }

    public static CreatureData LoadCreatureData(string creatureName)
    {
        string filePath = Path.Combine(creatureDataPath, creatureName + ".json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            CreatureData loadedData = ScriptableObject.CreateInstance<CreatureData>();
            JsonUtility.FromJsonOverwrite(json, loadedData);
            return loadedData;
        }
        else
        {
            Debug.LogError($"No CreatureData found at {filePath}");
            return null;
        }
    }

    public static void SaveGLB(GameObject glbModel, string creatureName)
    {
        string filePath = Path.Combine(glbDataPath, creatureName + ".glb");
        // This example assumes the GLB data is serialized and available in `glbModel`.
        byte[] glbData = GetGLBDataFromModel(glbModel);
        File.WriteAllBytes(filePath, glbData);
        Debug.Log($"Saved GLB model to {filePath}");
    }

    public static GameObject LoadGLB(string creatureName)
    {
        string filePath = Path.Combine(glbDataPath, creatureName + ".glb");
        if (File.Exists(filePath))
        {
            byte[] glbData = File.ReadAllBytes(filePath);
            return CreateGameObjectFromGLBData(glbData);  // Assuming a function for deserialization
        }
        else
        {
            Debug.LogError($"No GLB file found at {filePath}");
            return null;
        }
    }

    private static byte[] GetGLBDataFromModel(GameObject model)
    {
        // Serialize the GLB data here based on the model (requires GLTF exporter or custom serialization)
        // Placeholder implementation, replace with actual GLB serialization logic
        return new byte[0];
    }

    private static GameObject CreateGameObjectFromGLBData(byte[] data)
    {
        // Deserialize the GLB data here (requires GLTF importer or custom deserialization)
        // Placeholder implementation, replace with actual GLB import logic
        return new GameObject("ImportedModel");
    }
}
