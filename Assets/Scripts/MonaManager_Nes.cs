using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Monaverse.Api.Modules.User.Dtos;
using Monaverse.Modal;
using UnityEngine;
using UnityGLTF;

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
            currentModel = gltfComponent.gameObject.transform.GetChild(0).gameObject;
            
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
            Mesh mesh = model.GetComponentInChildren<MeshFilter>()?.sharedMesh 
             ?? model.GetComponentInChildren<SkinnedMeshRenderer>()?.sharedMesh;
    
            if (mesh == null)
            {
                Debug.LogWarning("No mesh found on the model.");
                return;
            }

            Bounds bounds = mesh.bounds;

            // Calculate the current size of the model in each dimension
            Vector3 currentSize = bounds.size;

            // Determine the maximum size of the model (assuming it's axis-aligned)
            float maxDimension = Mathf.Max(currentSize.x, currentSize.y, currentSize.z);

                // Add debug logs
            Debug.Log($"Current model size: {currentSize}, Max dimension: {maxDimension}");
            Debug.Log($"Before scaling - Model scale: {model.transform.localScale}");


            // Define the desired max and min sizes
            float desiredMaxSize = 0.6f; // 1 meter
            float desiredMinSize = 0.1f; // 0.1 meter

            // Calculate the scaling factor needed to resize the model
            float scaleFactor = Mathf.Clamp(desiredMaxSize / maxDimension, desiredMinSize / maxDimension, desiredMaxSize / maxDimension);

            // Apply the scaling factor to the model's transform
            model.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            Debug.Log($"After scaling - Model scale: {model.transform.localScale}");
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