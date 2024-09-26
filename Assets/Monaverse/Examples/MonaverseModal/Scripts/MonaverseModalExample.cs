using System.Collections.Generic;
using System.Threading.Tasks;
using Monaverse.Api.Modules.User.Dtos;
using Monaverse.Modal;
using UnityEngine;
using UnityGLTF;

namespace Monaverse.Examples
{
    public class MonaverseModalExample : MonoBehaviour
    {

        public GameObject currentlyLoadedModel = null;
        [SerializeField] private MonaCollectibleListExample _compatibleItems;
        [SerializeField] private MonaCollectibleItemExample _importedItem;
        private void Start()
        {
            MonaverseModal.ImportTokenClicked += OnImportTokenClicked;
            MonaverseModal.TokensLoaded += OnTokensLoaded;
            MonaverseModal.TokensViewOpened += OnTokensViewOpened;
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
            await _compatibleItems.SetCollectibles(loadedTokens);
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
        }

        private async void Load3DModel(string url, string tokenName)
        {
            // Load the 3D model from the given URL
            // find the GLTFComponent in the scene
            GLTFComponent gltfComponent = FindObjectOfType<GLTFComponent>();
            if (gltfComponent == null)
            {
                Debug.LogError("GLTFComponent not found in scene");
                return;
            }
            gltfComponent.GLTFUri = url;
            await gltfComponent.Load();
            if (gltfComponent.gameObject.transform.childCount == 0)
            {
                Debug.Log("No model found in GLTFComponent");
                return;
            }
            GameObject model = gltfComponent.gameObject.transform.GetChild(0).gameObject;
            ResizeModelToFit(model);
            //remove model from its parent
            model.transform.SetParent(null);
            model.name = tokenName;
            currentlyLoadedModel = model;
            currentlyLoadedModel.transform.position = new Vector3(0,0,-7f);
            
        }

        public void ResizeModelToFit(GameObject model)
        {
            // Get the MeshFilter component
            MeshFilter meshFilter = model.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null)
            {
                Debug.LogWarning("No MeshFilter found on the model.");
                return;
            }

            // Get the mesh bounds
            Bounds bounds = meshFilter.sharedMesh.bounds;

            // Calculate the current size of the model in each dimension
            Vector3 currentSize = bounds.size;

            // Determine the maximum size of the model (assuming it's axis-aligned)
            float maxDimension = Mathf.Max(currentSize.x, currentSize.y, currentSize.z);

            // Define the desired max and min sizes
            float desiredMaxSize = 1f; // 1 meter
            float desiredMinSize = 0.1f; // 0.1 meter

            // Calculate the scaling factor needed to resize the model
            float scaleFactor = Mathf.Clamp(desiredMaxSize / maxDimension, desiredMinSize / maxDimension, desiredMaxSize / maxDimension);

            // Apply the scaling factor to the model's transform
            model.transform.localScale = model.transform.localScale * scaleFactor;

            Debug.Log($"Model resized with scale factor: {scaleFactor}");
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