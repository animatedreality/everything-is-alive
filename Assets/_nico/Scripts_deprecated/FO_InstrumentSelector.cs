using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FO_InstrumentSelector : MonoBehaviour
{
    //get a list of target hand's colliders
    IList<OVRBoneCapsule> capsules = new List<OVRBoneCapsule>();

    //my InstrumentPrefab
    public FO_InstrumentPrefab myInstrumentPrefab;
    public GameObject collidingObject;
    public Renderer meshVisual;
    public Material selectedMat;
    public Material unselectedMat;
    // Start is called before the first frame update
    //added one line
    void Start()
    {
        meshVisual = transform.GetChild(0).GetComponent<Renderer>();
        meshVisual.material = unselectedMat;
    }

    // Update is called once per frame
    void Update()
    {
        capsules = FO_ControllerManager.s.rightSkeleton.Capsules;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collidingObject)
            return;
        foreach (var capsule in capsules)
        {
            if (capsule.CapsuleRigidbody.gameObject == collision.gameObject)
            {
                //Debug.Log("COLLISION HAPPENS" + capsule.CapsuleRigidbody.gameObject.transform.parent.name); 
                collidingObject = collision.gameObject;
                meshVisual.material = selectedMat;
                FO_InstrumentManager.s.SetCurrentInstrument(myInstrumentPrefab);
                break; // Exit the loop once a match is found
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collidingObject == collision.gameObject){
            meshVisual.GetComponent<Renderer>().material = unselectedMat;
            collidingObject = null;
        }
    }
}
