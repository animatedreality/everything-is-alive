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

        //Apply Materials
        Material defaultStandardMaterial = Resources.Load<Material>("GLTFMaterials/PBRGraph");
        //Material defaultMetallicMaterial = Resources.Load<Material>("GLTFMaterials/PBRMetallicRoughness");

        if (defaultStandardMaterial == null)
        {
            Debug.LogError("Default material could not be loaded from Resources. Make sure it's in Resources/GLTFMaterials/PBRGraph.");
            return loadedModel;
        }

        foreach (MeshRenderer renderer in loadedModel.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.sharedMaterial = defaultStandardMaterial;
        }

        Debug.Log($"Loaded and applied default material to model: {creatureName}");
        return loadedModel;
    }

    public static void SaveTexture(Texture2D texture, string fileName)
    {
        string texturePath = Path.Combine(glbDataPath, fileName); // Save in the same directory as GLB files

        byte[] textureBytes = texture.EncodeToJPG(); // Encode as JPG
        File.WriteAllBytes(texturePath, textureBytes);
        Debug.Log($"Texture saved at {texturePath}");
    }

    public static Sprite CreateSpriteFromBytes(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
        {
            Debug.LogWarning("No image data provided to create sprite.");
            return null;
        }

        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            var rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100.0f);
        }
        else
        {
            Debug.LogError("Failed to load image data into texture.");
            return null;
        }
    }

}
