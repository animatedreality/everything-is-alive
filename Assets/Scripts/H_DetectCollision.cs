using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class H_DetectCollision : MonoBehaviour
{
    public string targetStringContains = "Hand"; // name of target collision must contain this string
    public UnityEvent collisionEnterEvent, collisionExitEvent;
    public GameObject collidingObject;

    public float collisionBuffer = 0.1f; // Buffer time in seconds
    private float lastCollisionTime = -1f; // Time of the last collision

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (Time.time - lastCollisionTime < collisionBuffer)
        {
            return; // Exit if not enough time has passed since last collision
        }

        //Debug.Log(gameObject.name + "Colliding with " + collision.gameObject.name);
        if(collision.gameObject.name.Contains(targetStringContains)){
            Debug.Log("CollisionEnter");
            collidingObject = collision.gameObject;
            collisionEnterEvent?.Invoke();
            lastCollisionTime = Time.time;
            //call the CollisionEvent that can be overriden by children scripts
            CollisionEvent();
        }

    }

    protected virtual void CollisionEvent(){
        Debug.Log("Collision Event");
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log(gameObject.name + "Colliding with " + collision.gameObject.name);
        if(collision.gameObject == collidingObject){
            collisionExitEvent?.Invoke();
            collidingObject = null;
        }
    }
}
