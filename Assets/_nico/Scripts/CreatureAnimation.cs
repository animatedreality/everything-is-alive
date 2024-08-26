using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnimation : MonoBehaviour
{
    //Handles everything related to the creature's animation
    public Creature creature;
    [Header("All animators should use bool 'animate' to toggle states")]
    public List<Animator> animators = new List<Animator>();
    public bool isAnimating = false;

    // Start is called before the first frame update
    void Start()
    {
        StartAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAnimation(){
        foreach(Animator animator in animators){
            animator.SetBool("animate", true);
        }
        isAnimating = true;
    }

    public void StopAnimation(){
        foreach(Animator animator in animators){
            animator.SetBool("animate", false);
        }
        isAnimating = false;
    }
}