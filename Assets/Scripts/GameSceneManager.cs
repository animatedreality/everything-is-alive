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
        // Move this here instead of Awake
        if (OVRManager.instance != null)
        {
            OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.High;
        }

        StartCoroutine(InitializeScene());
    }

    private System.Collections.IEnumerator InitializeScene()
    {
        //Debug.Log("SceneManager: Starting initialization...");

        // Wait for all managers to be ready
        if (AudioManager.i == null || CreatureManager.i == null || UIManager.i == null)
        {
            Debug.LogError("SceneManager: AudioManager.i is NULL!");
            yield return null;
        }

        //Debug.Log("SceneManager: Calling AudioManager.Initialize()");
        AudioManager.i.Initialize();

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

        UIManager.i.Initialize();
        //Debug.Log("Scene initialization complete - UI should now show creature images");
    }

}
