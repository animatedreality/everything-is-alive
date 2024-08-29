using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_SmoothFollow : MonoBehaviour
{
    public Transform target;
    Vector3 targetPosition, currentPosition;
    public float smoothSpeed = 3f;
    public float smoothRotationSpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            targetPosition = target.position;
            currentPosition = transform.position;
            
            // Smoothly interpolate between the current position and the target position
            Vector3 smoothedPosition = Vector3.Lerp(currentPosition, targetPosition, smoothSpeed * Time.deltaTime);
            
            // Update the position of this object
            transform.position = smoothedPosition;

            // Optionally, you can also smooth the rotation
            Quaternion targetRotation = target.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
        }
    }
}
