using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Beat : MonoBehaviour
{
    public GameObject beatOnNoSound, beatOffNoSound;
    public GameObject beatOnHasSound, beatOffHasSound;
    public int beatIndex;
    public EA_Bar bar;
    public bool hasSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInitialized(int beatIndex, EA_Bar bar){
        gameObject.name = "Beat" + beatIndex + "of" + bar.gameObject.name;
        this.beatIndex = beatIndex;
        this.bar = bar;
    }

    public void SetSound(bool _hasSound){
        hasSound = _hasSound;
    }

    public void PlayBeat(bool _isOn){
        beatOnHasSound.SetActive(_isOn && hasSound);
        beatOnNoSound.SetActive(_isOn && !hasSound);
        beatOffHasSound.SetActive(!_isOn && hasSound);
        beatOffNoSound.SetActive(!_isOn && !hasSound);

        if(_isOn && hasSound){
            bar.instrumentNode.PlayNode();
        }
    }
}
