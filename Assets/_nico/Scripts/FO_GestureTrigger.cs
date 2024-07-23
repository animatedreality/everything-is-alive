using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FO_GestureTrigger : MonoBehaviour
{
    [SerializeField]
    public OVRHand hand;
    public UnityEvent OnPinchStart = new UnityEvent();
    private bool pinchStart = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hand.GetFingerIsPinching(OVRHand.HandFinger.Index)){
            if(!pinchStart){
                OnPinchStart.Invoke();
                pinchStart = true;
            }
        }
        else{
            pinchStart = false;
        }
    }


}
