using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    public float rightTrigger, leftTrigger;
    public bool rightTriggerDown, leftTriggerDown;

    public UnityEvent OnRightTriggerPressed;
    void Start()
    {
        if(OnRightTriggerPressed == null){
            OnRightTriggerPressed = new UnityEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // rightTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        // leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        // rightTriggerDown = rightTrigger > 0.5f;
        // leftTriggerDown = leftTrigger > 0.5f;
        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            OnRightTriggerPressed.Invoke();
        }
    }
}
