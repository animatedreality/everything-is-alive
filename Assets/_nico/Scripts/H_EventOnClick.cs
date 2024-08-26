using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class H_EventOnClick : MonoBehaviour
{
    public UnityEvent OnClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("ONCLICK")]
    private void OnMouseDown()
    {
        OnClick?.Invoke();
    }
}
