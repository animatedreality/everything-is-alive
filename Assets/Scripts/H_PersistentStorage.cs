using System.IO;
using UnityEngine;
using UnityGLTF;
using System.Threading.Tasks;
public static class H_PersistentStorage
{
    private static readonly string creatureDataPath = Path.Combine(Application.persistentDataPath, "CreatureData").Replace("\\", "/");
    private static readonly string glbDataPath = Path.Combine(Application.persistentDataPath, "GLBFiles").Replace("\\","/");

    private static readonly GLTFComponent gltfComponent;

    static H_PersistentStorage()
    {
        if (!Directory.Exists(creatureDataPath))
            Directory.CreateDirectory(creatureDataPath);

        if (!Directory.Exists(glbDataPath))
            Directory.CreateDirectory(glbDataPath);

        // Initialize a GameObject with GLTFComponent for GLB loading
        GameObject gltfLoaderObject = new GameObject("GLTFLoader");
        gltfComponent = gltfLoaderObject.AddComponent<GLTFComponent>();
        Object.DontDestroyOnLoad(gltfLoaderObject);  // Keep GLTFComponent available across scenes
    }

    public static void SaveNewCreature(CreatureData creatureData, GameObject glbModel, Vector3 glbModelScale){
        SaveCreatureData(creatureData);
        SaveGLB(glbModel, glbModelScale, creatureData.name);
        SaveCreatureScale(glbModelScale, creatureData.name);
        Debug.Log($"Saved new creature with name: {creatureData.name}");
    }
    
    public static async Task<GameObject> LoadNewCreatureModel(string creatureName){
        GameObject loadedGLBModel = await LoadGLB(creatureName);
        Vector3 loadedGLBModelScale = LoadCreatureScale(creatureName);
        loadedGLBModel.transform.localScale = loadedGLBModelScale;
        Debug.Log($"Loaded new creature with name: {creatureName}");
        return loadedGLBModel;
    }

    private static void SaveCreatureScale(Vector3 scale, string creatureName)
    {
        string scalePath = Path.Combine(creatureDataPath, creatureName + "_scale.json");
        string scaleJson = JsonUtility.ToJson(scale);
        File.WriteAllText(scalePath, scaleJson);
        Debug.Log($"Saved scale data for {creatureName} at {scalePath}");
    }

    private static Vector3 LoadCreatureScale(string creatureName)
    {
        string scalePath = Path.Combine(creatureDataPath, creatureName + "_scale.json");
        if (File.Exists(scalePath))
        {
            string scaleJson = File.ReadAllText(scalePath);
            return JsonUtility.FromJson<Vector3>(scaleJson);
        }
        Debug.LogError($"No scale data found for {creatureName} at {scalePath}");
        return Vector3.one; // default scale if not found
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

    public static void SaveGLB(GameObject glbModel, Vector3 glbModelScale, string creatureName)
    {
        if (glbModel == null)
        {
            Debug.LogError("GLB Model is null!");
            return;
        }
        string filePath = Path.Combine(glbDataPath, creatureName + ".glb").Replace("\\","/");
        string fileName = creatureName + ".glb";

        try
        {
            var exporter = new GLTFSceneExporter(
                new Transform[] { glbModel.transform },
                (texture) => texture.name
            );

            // Pass both the filename and filepath to SaveGLB
            exporter.SaveGLB(fileName, filePath);
            Debug.Log("Save GLB Filepath is " + filePath);

            // Verify file was created
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                Debug.Log($"GLB file created successfully. Size: {fileInfo.Length} bytes");
            }
            else
            {
                Debug.LogError("GLB file was not created!");
            }

            Debug.Log($"Saved GLB model to {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving GLB: {e.Message}\n{e.StackTrace}");
        }
    }

    public static async Task<GameObject> LoadGLB(string creatureName)
    {
        string filePath = Path.Combine(glbDataPath, creatureName + ".glb");
        if (!File.Exists(filePath))
        {
            Debug.LogError($"No GLB file found at {filePath}");
            return null;
        }

        // Configure GLTFComponent as in MonaManager_Nes
        gltfComponent.GLTFUri = filePath;

        Debug.Log($"Loading GLB model from {filePath}");
        await gltfComponent.Load();

        // Retrieve the loaded model as the first child
        if (gltfComponent.transform.childCount == 0)
        {
            Debug.LogError("No model found in GLTFComponent after loading.");
            return null;
        }

        GameObject loadedModel = gltfComponent.transform.GetChild(0).gameObject;

        return loadedModel;
    }

}
