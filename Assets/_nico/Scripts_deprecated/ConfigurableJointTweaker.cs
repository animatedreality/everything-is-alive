using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurableJointTweaker : MonoBehaviour
{
    List<ConfigurableJoint> joints = new List<ConfigurableJoint>();
    List<Rigidbody> rgbodies = new List<Rigidbody>();
    public float positionSpringAdjust = 1.2f;
    public float positionDamperAdjust = 1.0f;
    public float rotationSpringAdjust = 1.0f;
    public float rotationDamperAdjust = 1.0f;
    public float dragAdjust = 1.0f;
    public float angularDragAdjust = 1.0f;

    //adjust angle
    public float rotationLimitAdjust = 0.5f;

    void Start()
    {
        //get all the ConfigurableJoint component under this object's children
        joints = new List<ConfigurableJoint>(GetComponentsInChildren<ConfigurableJoint>());
        foreach (ConfigurableJoint joint in joints)
        {
            AdjustJointDampingValues(joint, positionSpringAdjust, positionDamperAdjust, rotationSpringAdjust, rotationDamperAdjust);
            AdjustJointRotationLimits(joint, rotationLimitAdjust);
        }
        foreach(Rigidbody rb in rgbodies)
        {
            AdjustRigidbodyDrag(rb, dragAdjust, angularDragAdjust);
        }
    }

     public void AdjustJointRotationLimits(ConfigurableJoint joint, float limitAdj)
    {
        SoftJointLimit lowAngularX = joint.lowAngularXLimit;
        SoftJointLimit highAngularX = joint.highAngularXLimit;
        SoftJointLimit angularY = joint.angularYLimit;
        SoftJointLimit angularZ = joint.angularZLimit;

        lowAngularX.limit *= limitAdj;
        highAngularX.limit *= limitAdj;
        angularY.limit *= limitAdj;
        angularZ.limit *= limitAdj;

        joint.lowAngularXLimit = lowAngularX;
        joint.highAngularXLimit = highAngularX;
        joint.angularYLimit = angularY;
        joint.angularZLimit = angularZ;
    }

    public void AdjustJointDampingValues(ConfigurableJoint joint, float posSpringAdj, float posDampAdj, float rotSpringAdj, float rotDampAdj)
    {
        // Adjust Position Drives
        JointDrive xDrive = joint.xDrive;
        JointDrive yDrive = joint.yDrive;
        JointDrive zDrive = joint.zDrive;

        xDrive.positionSpring *= posSpringAdj;
        xDrive.positionDamper *= posDampAdj;

        yDrive.positionSpring *= posSpringAdj;
        yDrive.positionDamper *= posDampAdj;

        zDrive.positionSpring *= posSpringAdj;
        zDrive.positionDamper *= posDampAdj;

        joint.xDrive = xDrive;
        joint.yDrive = yDrive;
        joint.zDrive = zDrive;

        // Adjust Rotation Drives
        JointDrive angularXDrive = joint.angularXDrive;
        JointDrive angularYZDrive = joint.angularYZDrive;
        JointDrive slerpDrive = joint.slerpDrive;

        angularXDrive.positionSpring *= rotSpringAdj;
        angularXDrive.positionDamper *= rotDampAdj;

        angularYZDrive.positionSpring *= rotSpringAdj;
        angularYZDrive.positionDamper *= rotDampAdj;

        slerpDrive.positionSpring *= rotSpringAdj;
        slerpDrive.positionDamper *= rotDampAdj;

        joint.angularXDrive = angularXDrive;
        joint.angularYZDrive = angularYZDrive;
        joint.slerpDrive = slerpDrive;
    }

    public void AdjustRigidbodyDrag(Rigidbody rb, float dragAdj, float angularDragAdj)
    {
        rb.linearDamping *= dragAdj;
        rb.angularDamping *= angularDragAdj;
    }
}
