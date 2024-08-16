using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAnimation_NS : MonoBehaviour
{
    //create a list of animators
    public List<Animator> animators = new List<Animator>();
    //---!!!!MAKE SURE ALL ANIMATORS HAVE THE 'animate' BOOL!!!!---
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleAnimation(){
        foreach (Animator animator in animators)
        {
            //play and stop animation
            if (animator.GetBool("animate"))
            {
                animator.SetBool("animate", false);
                //animator.Play("Idle", 0, 0f);
            }
            else
            {
                animator.SetBool("animate", true);
                //animator.Play("Play", 0, 0f);
            }
        }
    }
}
