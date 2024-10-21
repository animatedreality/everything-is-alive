using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Samples;

public class InputManager : MonoBehaviour
{
    bool rightControllerBButton, rightControllerAButton;
    bool leftControllerXButton;
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
            RightControllerBButton();
        }
        if(rightControllerAButton){
            RightControllerAButton();
        }
        if(leftControllerXButton){
            LeftControllerXButton();
        }
    }

    void RightControllerBButton(){
        //Spawn Creature if is in Game
        Debug.Log("RightControllerBButton");
        if(SceneManager.i.currentSceneState == SceneState.INGAME){
            CreatureManager.i.SpawnCreature();
        }
    }

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
