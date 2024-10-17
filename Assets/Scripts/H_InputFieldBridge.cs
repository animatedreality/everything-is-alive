using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class H_InputFieldBridge : MonoBehaviour
{
    public TMP_InputField TMP_inputField;
    public InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //whatever value is in the inputField, put it in the TMP_inputField
        TMP_inputField.text = inputField.text;
    }
}
