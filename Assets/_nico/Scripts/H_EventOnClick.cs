using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class H_EventOnClick : MonoBehaviour
{
    public Collider collider;
    public UnityEvent OnClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnMouseDown()
    {
        if (collider != null && collider.enabled)
        {
            OnClick?.Invoke();
        }
    }
}
