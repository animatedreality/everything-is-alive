using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Monaverse.Api.Modules.User.Dtos;
using Monaverse.Modal;
using UnityEngine;
using UnityGLTF;
using System.IO;
using System.Threading.Tasks;

namespace Monaverse.Examples
{
    public class MonaManager_Nes : MonoBehaviour
    {
                //model will be loaded into this object
        public GLTFComponent gltfComponent;
        public float scaleFactor = 0.1f;
        public GameObject currentModel;
        private void Start()
        {
            MonaverseModal.ImportTokenClicked += OnImportTokenClicked;
            MonaverseModal.TokensLoaded += OnTokensLoaded;
            MonaverseModal.TokensViewOpened += OnTokensViewOpened;
            //StartCoroutine(StartMonaModelForTesting());
        }

        IEnumerator StartMonaModelForTesting()
        {
            yield return new WaitForSeconds(2);
            MonaverseModal.Open();
        }

        public void StartMonaModel(){
            MonaverseModal.Open();
        }

        private void OnTokensViewOpened(object sender, List<TokenDto> tokens)
        {
            Debug.Log("[MonaverseModalExample.OnTokensViewOpened] loaded " + tokens.Count + " tokens");
        }

        /// <summary>
        /// Called when tokens are loaded from the Monaverse API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="loadedTokens">A list of loaded tokens</param>
        private async void OnTokensLoaded(object sender, List<TokenDto> loadedTokens)
        {
            Debug.Log("[MonaverseModalExample.OnTokensLoaded] loaded " + loadedTokens.Count + " tokens");
        }

        /// <summary>
        /// Called when the import button is clicked in a token details view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="token">The token selected for import</param>
        private void OnImportTokenClicked(object sender, TokenDto token)
        {
            Debug.Log("[MonaverseModalExample.OnImportTokenClicked] " + token.Name);
            Dictionary<string, string> files = new Dictionary<string, string>();
            //for each file in token.Files add the Url and Filetype
            foreach (var file in token.Files)
            {
                files.Add(file.Filetype, file.Url);
                Debug.Log(file.Filetype + " " + file.Url);
                if (file.Filetype == "glb")
                {
                    Load3DModel(file.Url, token.Name);
                }
            }
            if (files.Count == 0)
            {
                Debug.Log("No url files found for token: " + token.Name);
            }
            MonaverseModal.Open();
        }


        //Saving and Loading 3D Models

        public void SaveNewCreature(CreatureData creatureData, GameObject glbModel, Vector3 glbModelScale){
            SaveCreatureData(creatureData);
            //the glbModel here is the currentModel from Load3DModel below
            Save3DModelPersistently(glbModel, creatureData.name);
            SaveCreatureScale(glbModelScale, creatureData.name);
            Debug.Log($"Saved new creature with name: {creatureData.name}");
        }
        private async void Load3DModel(string url, string tokenName)
        {
            gltfComponent.GLTFUri = url;
            gltfComponent.AppendStreamingAssets = false;
            gltfComponent.UseStream = true;
            Debug.Log("NES_Loading model from: " + url);
            await gltfComponent.Load();
            //if (gltfComponent.gameObject.transform.childCount == 0)
            if (gltfComponent.gameObject.transform.childCount == 0)
            {
                Debug.Log("No model found in GLTFComponent");
                return;
            }
            currentModel = gltfComponent.transform.GetChild(0).gameObject;
            
            ResizeModelToFit(currentModel);
            currentModel.name = tokenName;
            currentModel.name += "_MonaModel";

            CreatureManager.i.CreateTempMonaCreature(currentModel, transform.position);

            UIManager.i.InitializeAudioClipsContainer();
        }

        public void ResizeModelToFit(GameObject model)
        {
            Debug.Log("NES_Resizing model to fit");
            // Get the MeshFilter component
            var renderer = model.GetComponentInChildren<MeshRenderer>() as Renderer
                ?? model.GetComponentInChildren<SkinnedMeshRenderer>() as Renderer;
    
            if (renderer == null)
            {
                Debug.LogWarning("No mesh found on the model.");
                return;
            }

            Bounds bounds = renderer.bounds;

            // Calculate the current size of the model in each dimension
            Vector3 currentSize = bounds.size;

            // Determine the maximum size of the model (assuming it's axis-aligned)
            float maxDimension = Mathf.Max(currentSize.x, currentSize.y, currentSize.z);

                // Add debug logs
            Debug.Log($"Current model size: {currentSize}, Max dimension: {maxDimension}");
            Debug.Log($"Before scaling - Model scale: {model.transform.localScale}");


            // Define the desired max and min sizes
            float desiredMaxSize = 0.3f; // 1 meter
            float desiredMinSize = 0.1f; // 0.1 meter

            // Calculate the scaling factor needed to resize the model
            float scaleFactor = Mathf.Clamp(desiredMaxSize / maxDimension, desiredMinSize / maxDimension, desiredMaxSize / maxDimension);

            // Apply the scaling factor to the model's transform
            model.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            Debug.Log($"After scaling - Model scale: {model.transform.localScale}");
        }

        private string GetPersistentModelPath(string modelName)
        {
            return Path.Combine(Application.persistentDataPath, modelName + ".glb");
        }

        public void Save3DModelPersistently(GameObject model, string _name)
        {
            string filePath = GetPersistentModelPath(_name);

            try
            {
                // Convert model to byte array using MemoryStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    var exporter = new GLTFSceneExporter(
                        new Transform[] { model.transform },
                        (texture) => texture.name
                    );
                    
                    // Export to memory stream instead of file
                    exporter.SaveGLBToStream(memoryStream, _name);
                    
                    // Write the bytes to file
                    File.WriteAllBytes(filePath, memoryStream.ToArray());
                    Debug.Log($"Model saved as bytes at {filePath}");
                }
                // // Initialize GLTF exporter with both required parameters
                // var exporter = new GLTFSceneExporter(
                //     new Transform[] { model.transform },
                //     (texture) => texture.name
                // );
                
                // // Export as GLB with filename and filepath
                // exporter.SaveGLB(_name + ".glb", filePath);

                // Debug.Log($"Model saved persistently at {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving GLB: {e.Message}\n{e.StackTrace}");
            }
        }

        public async Task<GameObject> Load3DModelPersistently(string modelName)
        {
            string filePath = GetPersistentModelPath(modelName);

            if (!File.Exists(filePath))
            {
                Debug.LogError($"No saved model found at {filePath}");
                return null;
            }

            // Configure GLTFComponent to load the GLB model from the local file
            gltfComponent.GLTFUri = filePath;
            gltfComponent.AppendStreamingAssets = false;
            gltfComponent.UseStream = true;

            Debug.Log($"Loading model persistently from {filePath}");
            await gltfComponent.Load();

            // Verify that model loaded successfully
            if (gltfComponent.gameObject.transform.childCount == 0)
            {
                Debug.LogError("Failed to load model in GLTFComponent.");
                return null;
            }

            // Get the loaded model from the GLTFComponent’s hierarchy
            GameObject loadedModel = gltfComponent.gameObject.transform.GetChild(0).gameObject;
            loadedModel.name = modelName;
            
            Debug.Log($"Model loaded persistently with name: {loadedModel.name}");
            return loadedModel;
        }

        public void SaveCreatureData(CreatureData creatureData)
        {
            string json = JsonUtility.ToJson(creatureData);
            string filePath = Path.Combine(Application.persistentDataPath, creatureData.name + "_CreatureData.json");
            File.WriteAllText(filePath, json);
            Debug.Log($"Saved CreatureData to {filePath}");
        }

        public CreatureData LoadCreatureData(string creatureName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, creatureName + "_CreatureData.json");
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

        private void SaveCreatureScale(Vector3 scale, string creatureName)
        {
            string scalePath = Path.Combine(Application.persistentDataPath, creatureName + "_scale.json");
            string scaleJson = JsonUtility.ToJson(scale);
            File.WriteAllText(scalePath, scaleJson);
            Debug.Log($"Saved scale data for {creatureName} at {scalePath}");
        }

        public Vector3 LoadCreatureScale(string creatureName)
        {
            string scalePath = Path.Combine(Application.persistentDataPath, creatureName + "_scale.json");
            if (File.Exists(scalePath))
            {
                string scaleJson = File.ReadAllText(scalePath);
                return JsonUtility.FromJson<Vector3>(scaleJson);
            }
            Debug.LogError($"No scale data found for {creatureName} at {scalePath}");
            return Vector3.one; // default scale if not found
        }

        /// <summary>
        /// This is the entry point for the Monaverse Modal
        /// Called on button click
        /// </summary>
        public void OpenModal()
        {
            MonaverseModal.Open();
        }
    }
}