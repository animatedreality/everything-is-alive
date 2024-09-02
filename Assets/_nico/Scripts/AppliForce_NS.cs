using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppliForce_NS : MonoBehaviour
{
    private Rigidbody rb;
    // Force magnitude to be applied
    public float downForceMagnitude = 10f;
    public float addedForceMagnitude = 10f;
    public float maxRotationAngle = 45f;
    public Rigidbody alternativeTarget;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse click");
            // Create a ray from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits this GameObject's collider
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                // Simulate the collision by applying a force
                SimulateCollision();
            }
        }
    }

    [ContextMenu("SimulateCollision")]
    public void SimulateCollision()
    {
        Debug.Log("SimulateCollision");
        Vector3 forceDirection = Vector3.down;
        // Calculate a random direction for the force to simulate a collision
        Vector3 addedForce = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // Apply the force to the Rigidbody
        //rb.AddForce(forceDirection * downForceMagnitude + addedForce * addedForceMagnitude, ForceMode.Impulse);
        if(alternativeTarget){
            alternativeTarget.AddForce(forceDirection * downForceMagnitude + addedForce * addedForceMagnitude, ForceMode.Impulse);
        }else{
            rb.AddForce(forceDirection * downForceMagnitude + addedForce * addedForceMagnitude, ForceMode.Impulse);
        }

        //ROTATION
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(-maxRotationAngle, maxRotationAngle),
            Random.Range(-maxRotationAngle, maxRotationAngle),
            Random.Range(-maxRotationAngle, maxRotationAngle)
        );

        if (alternativeTarget)
        {
            alternativeTarget.MoveRotation(alternativeTarget.rotation * randomRotation);
        }
        else
        {
            rb.MoveRotation(rb.rotation * randomRotation);
        }
    }
}
