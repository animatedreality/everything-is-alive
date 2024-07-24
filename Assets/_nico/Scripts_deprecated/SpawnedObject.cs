using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObject : MonoBehaviour
{
    public Renderer meshRenderer;
    public int materialIndex; 

    private Vector3 finalScale;
    private Vector3 heightRange = new Vector2 (1.5f, 2f);
    public GameObject heightOffsetObject;
    
    //create an animation of growing of the object from 0 to finalScale
    private float growSpeed = 0.1f;
    private float growAmount = 0.1f;
    private bool growStart = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(growStart && transform.localScale.y < finalScale.y){
            transform.localScale = Vector3.Lerp(transform.localScale, finalScale, growSpeed);
        }else{
            growStart = false;
        }
    }

    public void InitializeSpawnedObject(Color _color, int _materialIndex)
    {
        finalScale = new Vector3(1, Random.Range(heightRange.x, heightRange.y), 1);
        //heightOffsetObject's height should be the opposite of finalScale's y, so that it doesn't get stretched within the parent
        heightOffsetObject.transform.localScale = new Vector3(1, 1/finalScale.y, 1);
        growStart = true;
        meshRenderer.materials[materialIndex].color = _color;
    }
}
