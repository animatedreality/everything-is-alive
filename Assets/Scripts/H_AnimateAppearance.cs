using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class H_AnimateAppearance : MonoBehaviour
{
    public bool autoDetectScale = true;
    public Vector3 defaultScale = Vector3.one;
    public float animationTime = 0.3f;
    public bool destroyOnAnimateOut = false;
    public bool hideOnStart = true;
    void Awake(){
        if(autoDetectScale){
            defaultScale = transform.localScale;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(hideOnStart){
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimateIn(){
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(defaultScale, animationTime);
    }

    public void AnimateOut(){
        transform.DOScale(Vector3.zero, animationTime).OnComplete(() => {
            if(destroyOnAnimateOut){
                Destroy(gameObject);
            }else{
                gameObject.SetActive(false);
            }
        });
    }
}
