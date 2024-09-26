using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_Selection : MonoBehaviour
{
    public bool isSelected = false;
    public GameObject[] selectedObj, unselectedObj;

    // Start is called before the first frame update
    void Start()
    {
        SetActive(isSelected);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleSelected(){
        isSelected = !isSelected;
        SetActive(isSelected);
    }

    public void SetSelected(bool _isSelected){
        isSelected = _isSelected;
        SetActive(isSelected);
    }

    void SetActive(bool _isActive){
        foreach (GameObject obj in selectedObj)
        {
            if(obj != null)
                obj.SetActive(_isActive);
        }
        foreach (GameObject obj in unselectedObj)
        {
            if(obj != null) 
                obj.SetActive(!_isActive);
        }
    }

    public void DebugButtonPressed(){
        Debug.Log("Button pressed" + gameObject.name);
    }
}
