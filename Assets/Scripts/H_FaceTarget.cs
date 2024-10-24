using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_FaceTarget : MonoBehaviour
{
    public Transform targetObject;
    public Vector3 positionOffset = new Vector3(0, 1.5f, 0);
    public Vector3 rotationOffset = new Vector3(0, 180f, 0);
    // Start is called before the first frame update
    public void SetFaceTarget()
    {
        gameObject.SetActive(true);
        Vector3 spawnPos = targetObject.position + positionOffset;
        transform.position = spawnPos;
        transform.LookAt(targetObject);
        transform.rotation *= Quaternion.Euler(rotationOffset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
