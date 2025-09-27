using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneState{
    WELCOME,
    INGAME,
    SELECTMODEL,
    MAKEINSTRUMENT
}
public class GameSceneManager : MonoBehaviour
{
    //this is what MenuManager dreamed to be
    [Tooltip("SceneManager controls order of everything, including Game and Audio Manager")]
    public static GameSceneManager i;
    private void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Replace your obsolete code with this:
        OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.High;
    }

    public SceneState currentSceneState = SceneState.WELCOME;
    private SceneState previousSceneState = SceneState.WELCOME;

    void Start()
    {
        StartCoroutine(InitializeScene());
    }

    //private System.Collections.IEnumerator InitializeScene()
    //{
    //    AudioManager.i.Initialize();

    //    // Wait for creature resources to be loaded
    //    while (!CreatureManager.resourcesLoaded)
    //    {
    //        yield return null;
    //    }

    //    UIManager.i.Initialize();
    //    Debug.Log("Scene initialization complete - UI should now show creature images");
    //}

    private System.Collections.IEnumerator InitializeScene()
    {
        Debug.Log("SceneManager: Starting initialization...");

        // Check if AudioManager exists
        if (AudioManager.i == null)
        {
            Debug.LogError("SceneManager: AudioManager.i is NULL!");
            yield break;
        }

        Debug.Log("SceneManager: Calling AudioManager.Initialize()");
        AudioManager.i.Initialize();

        Debug.Log("SceneManager: Waiting for CreatureManager resources...");

        // Check if CreatureManager exists
        if (CreatureManager.i == null)
        {
            Debug.LogError("SceneManager: CreatureManager.i is NULL!");
            yield break;
        }

        // Wait for creature resources to be loaded
        int waitFrames = 0;
        while (!CreatureManager.resourcesLoaded)
        {
            waitFrames++;
            if (waitFrames > 300) // 5 seconds at 60fps
            {
                Debug.LogError("SceneManager: Timeout waiting for CreatureManager resources!");
                yield break;
            }
            yield return null;
        }

        Debug.Log("SceneManager: CreatureManager resources loaded, calling UIManager.Initialize()");

        // Check if UIManager exists
        if (UIManager.i == null)
        {
            Debug.LogError("SceneManager: UIManager.i is NULL!");
            yield break;
        }

        UIManager.i.Initialize();
        Debug.Log("Scene initialization complete - UI should now show creature images");
    }

}
