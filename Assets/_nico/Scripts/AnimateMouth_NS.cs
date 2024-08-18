using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TO BE USED WITH AnimatewithSamples_NS
public class AnimateMouth_NS : MonoBehaviour
{
    public GameObject upperLip, lowerLip;
    public Vector3 upperLipOpenVec3, lowerLipOpenVec3;
    private Quaternion upperLipOpen, lowerLipOpen;
    private Quaternion upperLipClosed, lowerLipClosed;
    // Start is called before the first frame update
    void Start()
    {
        upperLipClosed = upperLip.transform.localRotation;
        lowerLipClosed = lowerLip.transform.localRotation;
        upperLipOpen = Quaternion.Euler(upperLipOpenVec3);
        lowerLipOpen = Quaternion.Euler(lowerLipOpenVec3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // if t = 0, the mouth is closed
    // if t = 1, the mouth is open
    //add a local multiplier to the t value to make mouth open larger
    public float mouthOpenMultiplier = 1.5f;
    public void LerpMouthAnimation(float t)
    {
        upperLip.transform.localRotation = Quaternion.Lerp(upperLipClosed, upperLipOpen, t * mouthOpenMultiplier);

        lowerLip.transform.localRotation = Quaternion.Lerp(lowerLipClosed, lowerLipOpen, t * mouthOpenMultiplier);
    }
}