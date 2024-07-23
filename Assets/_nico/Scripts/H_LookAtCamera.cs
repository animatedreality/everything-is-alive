using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class H_LookAtCamera : MonoBehaviour
{

    public Vector3 localDirection = new Vector3(0,0,-1f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(FO_ControllerManager.s.ovrCameraRig){
            //use localDirection as forward, make this transform rotate towards cameraObject
            transform.LookAt(FO_ControllerManager.s.ovrCameraRig.transform.position + localDirection);
        }
    }
}
