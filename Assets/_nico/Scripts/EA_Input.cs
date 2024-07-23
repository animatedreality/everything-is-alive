using Meta.XR.MRUtilityKit;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;

public class EA_Input : MonoBehaviour
{


    public OVRSkeleton skeleton;
    public OVRHand hand;
    public bool onHandFound = false;
    //----- COLLIDERS -----
    [Header("Colliders")]
    public List<Collider> skeletonColliders = new List<Collider>();
    public List<string> skeletonColliderNames = new List<string>();
    public List<Transform> collidingObjects = new List<Transform>();
    public UnityEvent onCollision;

    //----- RAYCASTING -----
    [Header("Raycasting")]
    public Transform linePointer;
    public GameObject hitIndicator;
    public MRUKAnchor hitAnchor;
    private OVRSkeleton.BoneId indexTipBoneId = OVRSkeleton.BoneId.Hand_IndexTip;
    private OVRSkeleton.BoneId thumbRootBoneId = OVRSkeleton.BoneId.Hand_Thumb0;
    private OVRSkeleton.BoneId thumbTipBoneId = OVRSkeleton.BoneId.Hand_ThumbTip;
    private Transform indexTip, thumbRoot, thumbTip;


    //----- Hand & Controller Input -----
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hand.IsTracked && !onHandFound){
            OnHandFound();
            onHandFound = true;
        }
        if(!hand.IsTracked && onHandFound){
            OnHandLost();
            onHandFound = false;
        }

        if(onHandFound){
            PerformRaycast();
        }
    }

    public void OnHandFound(){
        skeletonColliders.Clear();
        skeletonColliderNames.Clear();
        foreach(OVRBoneCapsule capsule in skeleton.Capsules){
            skeletonColliders.Add(capsule.CapsuleCollider);
            skeletonColliderNames.Add(capsule.CapsuleCollider.gameObject.name);
            //check if CapsuleCollider already has a rigidbody, add one if it doesn't
            Rigidbody rb = capsule.CapsuleCollider.gameObject.GetComponent<Rigidbody>();
            if(rb == null){
                rb = capsule.CapsuleCollider.gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;
            rb.useGravity = false;
            //change tag of gameObject of the colliders
            capsule.CapsuleCollider.gameObject.tag = "XRHand";
        }

        if(skeletonColliders.Count > 0){
            // Add collision detection handlers
            foreach(CapsuleCollider capsule in skeletonColliders){
                //check if capsule.gameObject already have ColliderEventHandler
                if(capsule.gameObject.GetComponent<ColliderEventHandler>() == null){
                    ColliderEventHandler handler = capsule.gameObject.AddComponent<ColliderEventHandler>();
                    List<GameObject> colliderGameObjects = skeletonColliders.ConvertAll(collider => collider.gameObject);
                    handler.Initialize("", OnHandCollision, colliderGameObjects);
                }else{
                    Debug.Log("Handler already exists");
                }
            }
        }
    }

    private void OnHandCollision(GameObject collisionGameObject){
        //trigger onCollision unity event
        Debug.Log("Yayyy hand collided!" + collisionGameObject);
        onCollision.Invoke();
    }

    public void OnHandLost(){
        //skeletonColliders.Clear();
    }

    public void PerformRaycast(){
        indexTip = GetBoneTransform(skeleton, indexTipBoneId);
        thumbRoot = GetBoneTransform(skeleton, thumbRootBoneId);
        thumbTip = GetBoneTransform(skeleton, thumbTipBoneId);
        if(indexTip != null && thumbRoot != null && thumbTip != null){
            //Debug.Log("CREATING RAYCAST CONDITION");
            Vector3 medianTipPos = (indexTip.position + thumbTip.position) / 2;
            Vector3 pointingDirection = (medianTipPos - thumbRoot.position).normalized;
            //align line pointer
            linePointer.position = medianTipPos;
            linePointer.rotation = Quaternion.LookRotation(pointingDirection);
            //create Ray
            Ray ray = new Ray(medianTipPos, pointingDirection);
            hitAnchor = null;
            foreach(MRUKAnchor mrukAnchor in EA_XRData.s.mrukAnchors){
                if(mrukAnchor.Raycast(ray, 1000f, out RaycastHit hitInfo)){
                    //Debug.Log("RAYCAST HIT");
                    //hitIndicator.SetActive(true);
                    hitAnchor = mrukAnchor;
                    hitIndicator.transform.position = Vector3.Lerp(hitIndicator.transform.position, hitInfo.point, 0.1f);
                    break;
                }
            }
        }
    }

    Transform GetBoneTransform(OVRSkeleton skeleton, OVRSkeleton.BoneId boneId)
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



// Helper class to handle collider events and call the parent method
public class ColliderEventHandler : MonoBehaviour
{
    public string targetStringContains;
    private System.Action<GameObject> callOnCollision;
    public List<GameObject> avoidCollidingObjects = new List<GameObject>();

    public void Initialize(string targetStringContains, System.Action<GameObject> callOnCollision, List<GameObject> avoidCollidingGameObjects)
    {
        this.targetStringContains = targetStringContains;
        this.callOnCollision = callOnCollision;
        this.avoidCollidingObjects = avoidCollidingGameObjects;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected with: " + collision.gameObject.name);
        //do not invoke if the object is already in the list
        if (avoidCollidingObjects.Contains(collision.gameObject))
        {
            Debug.Log("Colliding with Self: " + collision.gameObject.name);
            return;
        }

        if (collision.gameObject.name.Contains(targetStringContains))
        {
            callOnCollision?.Invoke(collision.gameObject);
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     //do not invoke if the object is already in the list
    //     if(avoidCollidingObjects.Contains(other.gameObject)){
    //         return;
    //     }
    //     if (other.gameObject.name.Contains(targetStringContains))
    //     {
    //         //Debug.Log("Triggering with " + other.gameObject.name);
    //         callOnCollision?.Invoke(gameObject);
    //     }
    // }
}
