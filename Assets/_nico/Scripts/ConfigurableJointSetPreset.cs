using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Presets;

//require ConfigurableJoint component
[RequireComponent(typeof(ConfigurableJoint))]
public class ConfigurableJointSetPreset : MonoBehaviour
{

    public Preset preset;
    public bool assignConnectedBody = true;
    // Start is called before the first frame update
    void Start()
    {
        ApplyPreset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyPreset()
    {
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        preset.ApplyTo(joint);

        if(transform.parent.GetComponent<Rigidbody>() && assignConnectedBody){
            joint.connectedBody = transform.parent.GetComponent<Rigidbody>();
        }
    }
}
