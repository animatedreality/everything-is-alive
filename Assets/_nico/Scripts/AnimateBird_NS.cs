using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateBird_NS : MonoBehaviour
{
    //NECK
    public GameObject neckBone;
    public Vector3 neckRotMin, neckRotMax;
    public float neckRotSpeedMin, neckRotSpeedMax;
    public bool isAnimating = false;
    private Quaternion neckRotTarget;
    private float neckRotSpeed;

    //EYES
    public Material eyeOpenMat, eyeClosedMat;
    public GameObject eyeL, eyeR;
    
    void Start()
    {
        neckRotTarget = Quaternion.Euler(Vector3.Lerp(neckRotMin, neckRotMax, Random.Range(0f, 1f)));
        neckRotSpeed = Random.Range(neckRotSpeedMin, neckRotSpeedMax);
        SetEyes(isAnimating);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimating)
        {
            AnimateNeck();
        }
    }

    void AnimateNeck(){
        //randomly rotate neck between neckRotMin and neckRotMax over neckRotSpeedMin and neckRotSpeedMax
        //lerp neckBone rotation to neckRotTarget at a random speed between neckRotSpeedMin and neckRotSpeedMax, when target hit, set a new target
        neckBone.transform.localRotation = Quaternion.Lerp(neckBone.transform.localRotation, neckRotTarget, neckRotSpeed * Time.deltaTime);
        if (Quaternion.Angle(neckBone.transform.localRotation, neckRotTarget) < 0.1f)
        {
            
            //set a new target rot between neckRotMin and neckRotMax
            neckRotTarget = Quaternion.Euler(Vector3.Lerp(neckRotMin, neckRotMax, Random.Range(0f, 1f)));
            neckRotSpeed = Random.Range(neckRotSpeedMin, neckRotSpeedMax);
            Debug.Log("Target hit" + neckRotTarget);

        }

    }

    public void ToggleAnimation(){
        isAnimating = !isAnimating;
        SetEyes(isAnimating);
    }

    void SetEyes(bool isOpen){
        eyeL.GetComponent<MeshRenderer>().material = isOpen ? eyeOpenMat : eyeClosedMat;
        eyeR.GetComponent<MeshRenderer>().material = isOpen ? eyeOpenMat : eyeClosedMat;
    }
}
