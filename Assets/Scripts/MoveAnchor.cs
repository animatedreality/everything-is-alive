using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MoveAnchor : MonoBehaviour
{
    public Slider volumeSlider;
    public UnityEvent<float> onValueChanged = new UnityEvent<float>();
    // Start is called before the first frame update
    void Start()
    {
        //volumeSlider.onValueChanged.AddListener (ValueChangeCheck);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ValueChangeCheck(float value)
	{
        //onValueChanged.Invoke(value);
	}
}
