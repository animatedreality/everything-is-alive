using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Monaverse.Examples;
using UnityEngine.Pool;
using System.Linq;
using System.Reflection;



public class CreatureManager : MonoBehaviour
{
    [Header("Memory Management")]
    private List<CreatureData> tempCreatureDataInstances = new List<CreatureData>();

    public static CreatureManager i;

    // Cache all resources at startup
    private static Dictionary<string, CreatureData> cachedCreatureData = new Dictionary<string, CreatureData>();
    public static bool resourcesLoaded = false;

    private void Awake()
    {
        //Debug.Log("CreatureManager: Awake() called");

        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
            //Debug.Log("CreatureManager: Singleton instance set");
        }
        else
        {
            //Debug.Log("CreatureManager: Duplicate instance destroyed");
            Destroy(gameObject);
            return;
        }

        if (!resourcesLoaded)
        {
            //Debug.Log("CreatureManager: Starting resource loading...");
            LoadAllResourcesOnce();
            // Don't set resourcesLoaded = true here!
        }
    }

    public bool isInGame = true;

    public float creatureScaleMultiplier = 3;
    //Load all CreatureData from Resources/CreatureData
    public List<CreatureData> creatureDataList;
    public GameObject creatureFamilyPrefab;
    public Transform creatureSpawnPoint;

    //assigned when the creature's UIButtonContainer SelectButton() is called
    public CreatureData selectedCreatureData;

    //tracks which creature is currently being selectd
    public CreatureFamily selectedCreatureFamily;

    [Header("Instantiating Creatures")]
    public CreatureData creatureDataTemplate;
    public GameObject creatureMeshPrefab;

    [Header("Mona related")]
    public MonaManager_Nes monaManager_Nes;
    public GameObject tempMona3DModel;
    public Vector3 tempMona3DModelScale;
    public CreatureFamily tempMonaCreatureFamily;//temporarily create a new creatureFamily when browsing Mona Models

    [Header("Object Pooling")]
    public ObjectPool<GameObject> creatureFamilyPool;
    public ObjectPool<GameObject> tempCreaturePool;
    private const int INITIAL_POOL_SIZE = 10;

    void Start()
    {
        InitializePools();
        StartCoroutine(PrewarmPoolsCoroutine());
    }

    private void InitializePools()
    {
        creatureFamilyPool = new ObjectPool<GameObject>(
            createFunc: () => {
                GameObject creature = Instantiate(creatureFamilyPrefab);
                SetupHandGrabInteractable(creature);
                return creature;
            },
            actionOnGet: creature => {
                creature.SetActive(true);
                ResetCreatureForPool(creature);
            },
            actionOnRelease: creature => {
                CleanupCreatureForPool(creature);
                creature.SetActive(false);
            },
            actionOnDestroy: creature => Destroy(creature),
            collectionCheck: true,
            defaultCapacity: INITIAL_POOL_SIZE,
            maxSize: 20
        );
    }

    private void SetupHandGrabInteractable(GameObject creature)
    {
        // Find the HandGrabInteractable component
        var handGrabInteractable = creature.GetComponentInChildren<Oculus.Interaction.HandGrab.HandGrabInteractable>();

        if (handGrabInteractable != null)
        {
            // Find or add required components
            Rigidbody rb = handGrabInteractable.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = handGrabInteractable.gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true; // Usually kinematic for hand grab objects
            }

            // Find or add a collider
            Collider col = handGrabInteractable.GetComponent<Collider>();
            if (col == null)
            {
                col = handGrabInteractable.gameObject.AddComponent<BoxCollider>();
                BoxCollider boxCol = col as BoxCollider;
                boxCol.size = Vector3.one * 0.2f; // Adjust size as needed
                boxCol.isTrigger = true; // Usually trigger for hand interactions
            }

            // Assign the collider to the HandGrabInteractable
            AssignCollidersToHandGrab(handGrabInteractable, col);
        }
    }

    private void AssignCollidersToHandGrab(Oculus.Interaction.HandGrab.HandGrabInteractable handGrab, Collider collider)
    {
        // Create an array instead of a list
        Collider[] collidersArray = new Collider[] { collider };

        // Use reflection to set the colliders field since it might be private
        var fieldInfo = handGrab.GetType().GetField("_colliders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fieldInfo != null)
        {
            fieldInfo.SetValue(handGrab, collidersArray);
            return;
        }

        // Try alternative approach - look for public property
        var propertyInfo = handGrab.GetType().GetProperty("Colliders");
        if (propertyInfo != null && propertyInfo.CanWrite)
        {
            propertyInfo.SetValue(handGrab, collidersArray);
            return;
        }

        // If neither worked, try common field names
        var collectorsField = handGrab.GetType().GetField("_collectors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (collectorsField != null)
        {
            collectorsField.SetValue(handGrab, collidersArray);
        }
    }


    private void ResetCreatureForPool(GameObject creature)
    {
        // Reset the creature's state when retrieved from pool
        CreatureFamily creatureFamily = creature.GetComponent<CreatureFamily>();
        if (creatureFamily != null)
        {
            // Reset selection state
            creatureFamily.isSelected = false;

            // Ensure HandGrabInteractable is properly set up
            var handGrabInteractable = creature.GetComponentInChildren<Oculus.Interaction.HandGrab.HandGrabInteractable>();
            if (handGrabInteractable != null)
            {
                handGrabInteractable.enabled = true;

                // Verify colliders are still assigned
                Collider col = handGrabInteractable.GetComponent<Collider>();
                if (col != null)
                {
                    AssignCollidersToHandGrab(handGrabInteractable, col);
                }
            }
        }
    }

    private void CleanupCreatureForPool(GameObject creature)
    {
        // Clean up the creature before returning to pool
        CreatureFamily creatureFamily = creature.GetComponent<CreatureFamily>();
        if (creatureFamily != null)
        {
            // Deselect if selected
            if (creatureFamily.isSelected)
            {
                creatureFamily.OnDeselect();
            }

            // Clear any dynamic content in mesh container
            if (creatureFamily.meshContainer != null)
            {
                // Remove any dynamically created mesh children (but keep the prefab mesh)
                for (int i = creatureFamily.meshContainer.childCount - 1; i >= 0; i--)
                {
                    Transform child = creatureFamily.meshContainer.GetChild(i);
                    if (child.name.Contains("_Loaded_Model")) // Only remove dynamically loaded models
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
    }
    private System.Collections.IEnumerator PrewarmPoolsCoroutine()
    {
        var tempList = new List<GameObject>(INITIAL_POOL_SIZE);

        // Spread prewarming across multiple frames to avoid hitches
        for (int i = 0; i < INITIAL_POOL_SIZE; i++)
        {
            tempList.Add(creatureFamilyPool.Get());

            // Yield every few iterations to spread the load
            if (i % 3 == 0)
            {
                yield return null;
            }
        }

        // Return all objects to pool
        foreach (var obj in tempList)
        {
            creatureFamilyPool.Release(obj);
        }

        //Debug.Log($"Prewarmed creature family pool with {INITIAL_POOL_SIZE} objects");
    }


    private void LoadAllResourcesOnce()
    {
        StartCoroutine(LoadAllResourcesCoroutine());
    }

    private System.Collections.IEnumerator LoadAllResourcesCoroutine()
    {
        //Debug.Log("CreatureManager: LoadAllResourcesCoroutine started");
        yield return null; // Wait one frame before starting

        CreatureData[] allCreatureData = null;
        bool loadingSuccessful = false;

        // Try to load resources outside the yield context
        try
        {
            //Debug.Log("CreatureManager: Attempting to load CreatureData resources...");
            allCreatureData = Resources.LoadAll<CreatureData>("CreatureData");
            //Debug.Log($"CreatureManager: Found {(allCreatureData != null ? allCreatureData.Length : 0)} CreatureData assets");
            loadingSuccessful = true;
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"Error loading resources: {e.Message}");
            resourcesLoaded = true; // Set to true even on failure to prevent infinite wait
            yield break;
        }

        // If loading was successful, process the data
        if (loadingSuccessful && allCreatureData != null && allCreatureData.Length > 0)
        {
            //Debug.Log($"CreatureManager: Processing {allCreatureData.Length} creature data assets");

            // Process in batches to avoid frame drops
            const int batchSize = 3;
            int processed = 0;

            for (int i = 0; i < allCreatureData.Length; i += batchSize)
            {
                int end = Mathf.Min(i + batchSize, allCreatureData.Length);
                for (int j = i; j < end; j++)
                {
                    cachedCreatureData[allCreatureData[j].name] = allCreatureData[j];
                    processed++;
                }

                // Yield every batch to maintain frame rate
                yield return null;
            }

            creatureDataList = new List<CreatureData>(allCreatureData);
            //Debug.Log($"CreatureManager: Loaded {processed} creature data assets across {Mathf.CeilToInt((float)allCreatureData.Length / batchSize)} frames");
            //Debug.Log($"CreatureManager: creatureDataList now has {creatureDataList.Count} items");
        }
        else
        {
            //Debug.LogError("CreatureManager: No CreatureData assets found or loading failed!");
            creatureDataList = new List<CreatureData>(); // Initialize empty list
        }

        //Set resourcesLoaded to true ONLY after loading completes
        resourcesLoaded = true;
        //Debug.Log("CreatureManager: Resource loading complete, setting resourcesLoaded = true");
    }



    private void OnDestroy()
    {
        CleanupTempCreatureData();

        // Clean up pools
        creatureFamilyPool?.Dispose();
        tempCreaturePool?.Dispose();
    }

    public CreatureData GetCachedCreatureData(string name)
    {
        return cachedCreatureData.TryGetValue(name, out CreatureData data) ? data : null;
    }

    public async Task SpawnCreatureAsync()
    {
        try
        {
            if (!AudioManager.i.globalPlay)
            {
                AudioManager.i.Play();
            }

            GameObject newCreature = null;

            if (selectedCreatureData != null && isInGame)
            {
                if (selectedCreatureData.name.StartsWith("MonaModel", System.StringComparison.Ordinal))
                {
                    newCreature = await CreateNewCreatureAsync(selectedCreatureData);
                }
                else
                {
                    newCreature = creatureFamilyPool.Get();

                    // Initialize the creature AFTER getting it from pool
                    CreatureFamily creatureFamily = newCreature.GetComponent<CreatureFamily>();
                    if (creatureFamily != null)
                    {
                        creatureFamily.Initialize(selectedCreatureData);
                    }
                }

                if (newCreature != null)
                {
                    newCreature.transform.position = creatureSpawnPoint.position;
                }
            }
        }
        catch (System.Exception e)
        {
           // Debug.LogError($"Error spawning creature: {e.Message}");
        }
    }


    public void ReturnCreatureToPool(GameObject creature)
    {
        creatureFamilyPool.Release(creature);
    }

    //Creating Creatures from Mona Models
    public async Task<GameObject> CreateNewCreatureAsync(CreatureData _creatureData)
    {
        // Keep the existing implementation but add try-catch
        try
        {
            selectedCreatureData = _creatureData;

            GameObject newCreatureModel = await monaManager_Nes.Load3DModelPersistently(_creatureData.name);

            GameObject newCreatureFamily = Instantiate(creatureFamilyPrefab);
            newCreatureFamily.GetComponent<CreatureFamily>().Initialize(_creatureData);

            newCreatureFamily.name = _creatureData.name + "_Loaded_Family";
            newCreatureModel.name += "_Loaded_Model";
            newCreatureModel.transform.parent = newCreatureFamily.GetComponent<CreatureFamily>().creatureMesh.GetComponentInChildren<CreatureMemberDefault>().transform;

            monaManager_Nes.ResizeModelToFit(newCreatureModel);

            // Batch transform operations
            newCreatureModel.transform.SetLocalPositionAndRotation(
                new Vector3(0, 0.04f, 0),
                Quaternion.identity
            );
            newCreatureModel.transform.localScale = Vector3.one * 0.333f;

            return newCreatureFamily;
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"Error creating new creature: {e.Message}");
            return null;
        }
    }

    public void SelectCreatureFamily(CreatureFamily _creatureFamily){
        if(selectedCreatureFamily != null && selectedCreatureFamily != _creatureFamily){
            selectedCreatureFamily.OnDeselect();
        }
        selectedCreatureFamily = _creatureFamily;
        //Debug.Log("Selected Creature Family: " + selectedCreatureFamily.name);
    }

    public void CreateTempMonaCreature(GameObject _model, Vector3 _position)
    {
        if (!AudioManager.i.globalPlay)
        {
            AudioManager.i.Play();
        }

        // Clean up any existing temp creature data
        CleanupTempCreatureData();

        CreatureData monaCreatureData = ScriptableObject.CreateInstance<CreatureData>();
        monaCreatureData.name = _model.name;
        monaCreatureData.sprite = creatureDataTemplate.sprite;
        monaCreatureData.audioClips = creatureDataTemplate.audioClips;
        monaCreatureData.creatureMemberCount = 1;
        monaCreatureData.creatureType = CreatureData.CreatureType.Drum;
        monaCreatureData.sequenceLengthMultiplier = 1;
        monaCreatureData.sequenceLength = 16;

        // Track this instance for cleanup
        tempCreatureDataInstances.Add(monaCreatureData);
        selectedCreatureData = monaCreatureData;

        GameObject monaCreatureFamilyObj = Instantiate(creatureFamilyPrefab);
        monaCreatureFamilyObj.transform.position = _position;
        monaCreatureFamilyObj.name = "CreatureFamily_" + _model.name;
        CreatureFamily monaCreatureFamily = monaCreatureFamilyObj.GetComponent<CreatureFamily>();

        // Batch transform operations
        _model.transform.SetParent(monaCreatureFamily.creatureMesh.GetComponentInChildren<CreatureMemberDefault>().transform);
        _model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 180, 0));

        tempMona3DModel = _model;
        tempMona3DModelScale = _model.transform.lossyScale;
        tempMonaCreatureFamily = monaCreatureFamilyObj.GetComponent<CreatureFamily>();

        monaCreatureFamily.meshContainer.transform.localPosition = new Vector3(0, 0.02f, 0);
    }

    private void CleanupTempCreatureData()
    {
        foreach (var data in tempCreatureDataInstances)
        {
            if (data != null)
            {
                DestroyImmediate(data);
            }
        }
        tempCreatureDataInstances.Clear();
    }

    //call this with a custom button when Mona object pops up and is selected
    public void SaveTempMonaCreature()
    {
        if (tempMonaCreatureFamily == null)
        {
            //Debug.LogWarning("SaveTempMonaCreature: No temporary creature family to save.");
            return;
        }

        if (tempMonaCreatureFamily.creatureData == null)
        {
            //Debug.LogWarning("SaveTempMonaCreature: Temporary creature family has no creature data.");
            return;
        }

        if (selectedCreatureData != tempMonaCreatureFamily.creatureData)
        {
            //Debug.LogWarning("SaveTempMonaCreature: Selected creature data does not match temporary creature family.");
            return;
        }

        if (tempMona3DModel == null)
        {
            //Debug.LogWarning("SaveTempMonaCreature: No temporary 3D model to save.");
            return;
        }

        monaManager_Nes.SaveNewCreature(selectedCreatureData, tempMona3DModel, tempMona3DModelScale);
        UIManager.i.AddNewCreatureButton(selectedCreatureData);

        // Clean up
        CleanupTempCreatureData();
        if (tempMonaCreatureFamily != null)
        {
            Destroy(tempMonaCreatureFamily.gameObject);
        }
        tempMonaCreatureFamily = null;
        selectedCreatureData = null;
    }

    void SetLocalScaleToMatchGlobalScale(Transform transform, Vector3 targetGlobalScale)
    {
        Transform parent = transform.parent;
        Vector3 newLocalScale = targetGlobalScale;

        // Divide the target global scale by each parent's global scale to get the necessary local scale
        while (parent != null)
        {
            newLocalScale.x /= parent.lossyScale.x;
            newLocalScale.y /= parent.lossyScale.y;
            newLocalScale.z /= parent.lossyScale.z;
            parent = parent.parent;
        }

        transform.localScale = newLocalScale;
    }
}
