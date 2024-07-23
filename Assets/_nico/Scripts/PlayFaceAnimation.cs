using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayFaceAnimation : MonoBehaviour
{
    Animator animator;
    public string[] singingStates;
    string idleState = "idle";
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnimation(string _state){
        if(!animator)
            return;
        Debug.Log("Play face animation: " + _state);
        if(_state == "sing"){
            animator.Play(singingStates[Random.Range(0, singingStates.Length)]);
        }else if(_state == "idle"){
            animator.Play(idleState);
        }
    }
}
