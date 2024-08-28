using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("Global Awake");
    }
    public GameObject leftMenu, rightMove, rightSelect, rightInstantiate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableLeftMenuHint(){
        leftMenu.SetActive(false);
    }

    public void DisableRightMoveHint(){
        rightMove.SetActive(false);
    }

    public void DisableRightSelectHint(){
        rightSelect.SetActive(false);
    }

    public void DisableRightInstantiateHint(){
        rightInstantiate.SetActive(false);
    }
}
