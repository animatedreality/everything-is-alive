using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_PositionAtPlayer : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(PositionAtListenerWithDelay());
    }

    private IEnumerator PositionAtListenerWithDelay()
    {
        yield return new WaitForSeconds(1f); // 1-second delay
        transform.GetChild(0).gameObject.SetActive(true);
        AudioListener listener = FindObjectOfType<AudioListener>();
        if(listener != null){
            Debug.Log("listener found" + listener.transform.position);
            transform.position = listener.transform.position;
            
            //position child to face listener
            transform.LookAt(listener.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
