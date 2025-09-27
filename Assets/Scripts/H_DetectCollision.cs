using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class H_DetectCollision : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Use tags for better performance than string matching")]
    public bool useTagInsteadOfName = true;

    [Tooltip("Target tag to detect (recommended)")]
    public string targetTag = "Hand";

    [Tooltip("Target string name contains (fallback, less efficient)")]
    public string targetStringContains = "Hand";

    [Header("Events")]
    public UnityEvent collisionEnterEvent, collisionExitEvent;

    [Header("Collision Settings")]
    public float collisionBuffer = 0.1f;

    // Cached references
    private GameObject collidingObject;
    private float lastCollisionTime = -1f;

    // Performance optimization: cache string hash for faster comparison
    private int targetStringHash;
    private bool hasTargetString;

    void Start()
    {
        // Cache string hash for faster string comparison if not using tags
        if (!useTagInsteadOfName && !string.IsNullOrEmpty(targetStringContains))
        {
            targetStringHash = targetStringContains.GetHashCode();
            hasTargetString = true;
        }

        // Validate tag exists if using tag-based detection
        if (useTagInsteadOfName)
        {
            try
            {
                GameObject.FindWithTag(targetTag);
            }
            catch (UnityException)
            {
                Debug.LogWarning($"Tag '{targetTag}' doesn't exist. Consider adding it to Tags & Layers.", this);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Early exit for collision buffer
        if (Time.time - lastCollisionTime < collisionBuffer)
            return;

        // Optimized target detection
        bool isTargetObject = false;

        if (useTagInsteadOfName)
        {
            // Tag comparison is much faster than string operations
            isTargetObject = collision.gameObject.CompareTag(targetTag);
        }
        else if (hasTargetString)
        {
            // Optimized string comparison using cached hash
            isTargetObject = IsTargetName(collision.gameObject.name);
        }

        if (isTargetObject)
        {
            collidingObject = collision.gameObject;
            lastCollisionTime = Time.time;

            // Invoke events
            collisionEnterEvent?.Invoke();
            CollisionEvent();

            // Optimized debug logging (only when needed)
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("CollisionEnter", this);
#endif
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Direct reference comparison is faster than null check + equality
        if (ReferenceEquals(collision.gameObject, collidingObject))
        {
            collisionExitEvent?.Invoke();
            collidingObject = null;
        }
    }

    // Optimized string comparison method
    private bool IsTargetName(string objectName)
    {
        // Fast path: exact match first
        if (objectName == targetStringContains)
            return true;

        // Fallback to Contains() if needed
        return objectName.Contains(targetStringContains);
    }

    protected virtual void CollisionEvent()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("Collision Event", this);
#endif
    }

    // Public methods for runtime switching (optional)
    public void SetTargetTag(string newTag)
    {
        targetTag = newTag;
        useTagInsteadOfName = true;
    }

    public void SetTargetString(string newString)
    {
        targetStringContains = newString;
        targetStringHash = newString.GetHashCode();
        hasTargetString = !string.IsNullOrEmpty(newString);
        useTagInsteadOfName = false;
    }
}
