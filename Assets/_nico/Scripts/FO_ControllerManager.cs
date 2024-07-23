using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

public class FO_ControllerManager : MonoBehaviour
{
    public static FO_ControllerManager s { get; private set; }
    private void Awake()
    {
        if (s == null)
        {
            s = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (s != this)
        {
            Destroy(gameObject);
        }
    }
    
    //XR RELATED
    public OVRHand leftHand, rightHand;
    public OVRSkeleton leftSkeleton, rightSkeleton;
    public OVRCameraRig ovrCameraRig;
    //get boneID from wrist
    private OVRSkeleton.BoneId wristBoneId = OVRSkeleton.BoneId.Hand_WristRoot;
    private Transform leftWristTransform, rightWristTransform;

    //UI RELATED
    public GameObject leftHandMenuContainer, rightHandMenuContainer;
    private bool spawnedLeftMenu, spawnedRightMenu;

    public bool isLeftHandOpenTowardsHead, isRightHandOpenTowardsHead;
    
    // Start is called before the first frame update
    void Start()
    {
        SetLeftHandInstrumentSelectorUI();
    }

    // Update is called once per frame
    void Update()
    {
        leftWristTransform = GetBoneTransform(leftSkeleton, wristBoneId);
        rightWristTransform = GetBoneTransform(rightSkeleton, wristBoneId);


        if(leftWristTransform != null){
            //trigger menu selector on and off
            SetInstrumentSelectorUIState(DetectOpenPalmTowardsHead(leftHand, leftSkeleton, leftWristTransform));
        }else{
            SetInstrumentSelectorUIState(false);
        }
        
    }

    //spawn instrumentSelectorUI object to be attached to the leftHand
    private void SetLeftHandInstrumentSelectorUI(){
        int instrumentAmount = FO_InstrumentManager.s.instrumentPrefabs.Length;
        float angleStep = 360.0f / instrumentAmount;
        float radius = 0.1f; // Set the radius of the circle

        for (int i = 0; i < instrumentAmount; i++)
        {   
            Debug.Log("Spawned instrument selector " + i);
            float angle = i * angleStep * Mathf.Deg2Rad; // Convert angle to radians
            Vector3 spawnPos = new Vector3(Mathf.Cos(angle) * radius + 0.07f, 0.07f, Mathf.Sin(angle) * radius);
            GameObject instrumentSelectorUI = FO_InstrumentManager.s.instrumentPrefabs[i].instrumentSelectorUI;
            instrumentSelectorUI.transform.parent = leftHandMenuContainer.transform;
            instrumentSelectorUI.transform.localPosition = spawnPos;
            instrumentSelectorUI.transform.localRotation = Quaternion.identity;
            instrumentSelectorUI.SetActive(true);
        }

        leftHandMenuContainer.SetActive(false);
    }

    private void SetInstrumentSelectorUIState(bool _show){
        if(_show){
            leftHandMenuContainer.SetActive(true);
            leftHandMenuContainer.transform.position = leftWristTransform.position;
        }else{
            leftHandMenuContainer.SetActive(false);
        }
    }



    private bool DetectOpenPalmTowardsHead(OVRHand hand, OVRSkeleton skeleton, Transform _wristTransform)
    {
        // if (hand.GetFingerIsPinching(OVRHand.HandFinger.Index) == false &&
        //     hand.GetFingerIsPinching(OVRHand.HandFinger.Middle) == false &&
        //     hand.GetFingerIsPinching(OVRHand.HandFinger.Ring) == false &&
        //     hand.GetFingerIsPinching(OVRHand.HandFinger.Pinky) == false &&
        //     hand.GetFingerIsPinching(OVRHand.HandFinger.Thumb) == false)
        // {
            //checkk if hand is towards head
            Vector3 handToHead = ovrCameraRig.centerEyeAnchor.position - _wristTransform.position;

            if (Vector3.Dot(_wristTransform.up, handToHead.normalized) > 0.5f) // Adjust the threshold as needed
            {
                return true;
            }else{
                return false;
            }
        // }
        return false;
    }

    public Transform GetBoneTransform(OVRSkeleton skeleton, OVRSkeleton.BoneId boneId)
    {
        if(skeleton == null) return null;
        foreach (var bone in skeleton.Bones)
        {
            if (bone.Id == boneId)
            {
                return bone.Transform;
            }
        }
        return null;
    }
}
