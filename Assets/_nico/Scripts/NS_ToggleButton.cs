using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NS_ToggleButton : MonoBehaviour
{
    public GameObject playButton, pauseButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleButton(bool isOn){
        playButton.SetActive(isOn);
        pauseButton.SetActive(!isOn);
    }
}
