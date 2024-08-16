using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateMovementwithSamples_NS : MonoBehaviour
{
    public float rotationSpeedMax = 3f;
    public float rotationSpeedMin = 0f;
    public Vector3 rotationDirection = new Vector3(0,1,0);
    public float rotationSpeed = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAnimation(float _t){
        rotationSpeed = Mathf.Lerp(rotationSpeedMin, rotationSpeedMax, _t);
        // Rotate the object based on the input parameter _t and rotationSpeed
        transform.Rotate(rotationSpeed * rotationDirection * _t);
    }
}
