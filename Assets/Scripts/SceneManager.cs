using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneState{
    WELCOME,
    INGAME,
    SELECTMODEL,
    MAKEINSTRUMENT
}
public class SceneManager : MonoBehaviour
{
    //this is what MenuManager dreamed to be
    [Tooltip("SceneManager controls order of everything, including Game and Audio Manager")]
    public static SceneManager i;
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
    }

    public SceneState currentSceneState = SceneState.WELCOME;
    private SceneState previousSceneState = SceneState.WELCOME;

    void Start()
    {
        //!!!!! DO NOT SHUFFLE ORDER !!!!!
        AudioManager.i.Initialize();
        CreatureManager.i.Initialize();
        UIManager.i.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
