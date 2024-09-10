using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_SetGridLayoutHeight : MonoBehaviour
{
    int childCount;
    int heightMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        //call SetHeight() with a 0.5s delay
        Invoke("SetHeight", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetHeight(){
        childCount = transform.childCount;
        int remainder = childCount % 3;
        heightMultiplier = childCount / 3 + (remainder > 0 ? 1 : 0);
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, heightMultiplier * 86f);
        Debug.Log("reset height: " + childCount + " " + heightMultiplier);
    }
}
