using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Samples;

public class InputManager : MonoBehaviour
{
    bool rightControllerBButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rightControllerBButton = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);

        if(rightControllerBButton){
            RightControllerBButton();
        }
    }

    void RightControllerBButton(){
        //Spawn Creature if is in Game
        Debug.Log("RightControllerBButton");
        if(SceneManager.i.currentSceneState == SceneState.INGAME){
            CreatureManager.i.SpawnCreature();
        }
    }
}
