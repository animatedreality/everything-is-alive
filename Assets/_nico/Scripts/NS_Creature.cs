using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NS_Creature : MonoBehaviour
{
    public GameObject moveAnchor, toggleButton;
    public NS_TogglePlay togglePlayScript; // this controls animation states of the creature
    public NS_ToggleButton toggleButtonScript;
    public bool isSinging = false;
    //- Drag around the floor
    //- toggle MoveAnchor and ToggleButton when raycasted
    void Start()
    {
        SetAnchorUIScale(0, 0);
        if(!toggleButtonScript){
            toggleButtonScript = toggleButton.GetComponent<NS_ToggleButton>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHoverStart(){
        Debug.Log("OnHoverStart");
        //tween moveAnchor and toggleButton scale to 1
        SetAnchorUIScale(1, 0.5f);
    }

    //OnHoverEnd is called when other object is selected
    public void OnHoverEnd(){
        Debug.Log("OnHoverEnd");
        // SetAnchorUIScale(0, 0.5f);
    }

    public void OnClick(){
        isSinging = !isSinging;
        toggleButtonScript.ToggleButton(isSinging);
        togglePlayScript.TriggerOnClick();
    }

    void SetAnchorUIScale(float scale, float duration){
        moveAnchor.transform.DOScale(scale, duration);
        //toggleButton.transform.DOScale(scale, duration);
    }
}
