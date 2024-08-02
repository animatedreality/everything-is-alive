using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Presets;

public class ConfigurableJointAdjust : MonoBehaviour
{
    [Header("Spring=Restriction, larger spring less angle movement")]
    public float angularDriveSpring = 40f;
    [Header("Damping=Friction, larger damp slower movement")]
    public float angularDriveDamping = 2f;

    //will get all of these from children elements
    public List<ConfigurableJoint> joints = new List<ConfigurableJoint>();
    public List<ConfigurableJointSetPreset> jointPresetScripts = new List<ConfigurableJointSetPreset>();
    public List<Rigidbody> rgbodies = new List<Rigidbody>();
    // Start is called before the first frame update
    void Start()
    {
        joints = new List<ConfigurableJoint>(GetComponentsInChildren<ConfigurableJoint>());
        // //apply presets first to all ConfigurableJoints

        // //assign connected body after applying for presets
        // foreach(ConfigurableJoint joint in joints){
        //     if(joint.transform.parent.GetComponent<Rigidbody>()){
        //         joint.connectedBody = joint.transform.parent.GetComponent<Rigidbody>();
        //     }
        // }

        //initiate all the ConfigurableJointSetPreset scripts
        // jointPresetScripts = new List<ConfigurableJointSetPreset>(GetComponentsInChildren<ConfigurableJointSetPreset>());
        // foreach(ConfigurableJointSetPreset jointPresetScript in jointPresetScripts){
        //     jointPresetScript.ApplyPreset();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        AdjustPhysics();
    }


    public void AdjustPhysics(){
        foreach (ConfigurableJoint joint in joints)
        {
            AdjustSpring(joint, angularDriveSpring);
            AdjustDamping(joint, angularDriveDamping);
        }
    }
    public void AdjustSpring(ConfigurableJoint joint, float spring)
    {
        JointDrive angularXDrive = joint.angularXDrive;
        JointDrive angularYZDrive = joint.angularYZDrive;
        angularXDrive.positionSpring = spring;
        angularYZDrive.positionSpring = spring;
        joint.angularXDrive = angularXDrive;
        joint.angularYZDrive = angularYZDrive;
    }

    public void AdjustDamping(ConfigurableJoint joint, float damping)
    {
        JointDrive angularXDrive = joint.angularXDrive;
        JointDrive angularYZDrive = joint.angularYZDrive;
        angularXDrive.positionDamper = damping;
        angularYZDrive.positionDamper = damping;
        joint.angularXDrive = angularXDrive;
        joint.angularYZDrive = angularYZDrive;
    }
}
