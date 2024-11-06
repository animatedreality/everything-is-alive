using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Samples;
using System.Threading.Tasks;

public class InputManager : MonoBehaviour
{
    bool rightControllerBButton, rightControllerAButton;
    bool leftControllerXButton;
    bool isSpawningCreature = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rightControllerBButton = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);
        rightControllerAButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
        leftControllerXButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);
        if(rightControllerBButton){
            StartCoroutine(RightControllerBButtonCoroutine());
        }
        if(rightControllerAButton){
            RightControllerAButton();
        }
        if(leftControllerXButton){
            LeftControllerXButton();
        }
    }

    private IEnumerator RightControllerBButtonCoroutine(){
        if(isSpawningCreature){
            yield break;
        }
        isSpawningCreature = true;
        Debug.Log("RightControllerBButton");
        if(SceneManager.i.currentSceneState == SceneState.INGAME){
            var task = CreatureManager.i.SpawnCreature();
            while (!task.IsCompleted)
                yield return null;
        }
        isSpawningCreature = false;
    }

    // async void RightControllerBButton(){
    //     //Spawn Creature if is in Game
    //     Debug.Log("RightControllerBButton");
    //     if(SceneManager.i.currentSceneState == SceneState.INGAME){
    //         CreatureManager.i.SpawnCreature();
    //     }
    // }

    void RightControllerAButton(){
        Debug.Log("RightControllerAButton");
        //Toggle Hint
        UIManager.i.ToggleGameplayHint();
    }

    void LeftControllerXButton(){
        Debug.Log("LeftControllerXButton");
        UIManager.i.ToggleMainMenu();
    }
}
