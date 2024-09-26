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
    }
    public GameObject leftMenu, rightMove, rightSelect, rightInstantiate, tutorialToggleQuestionmark;
    public bool isShowingTutorial = true;
    bool flipTutorialButton = false;
    // Start is called before the first frame update
    void Start()
    {
        SetShowTutorial(false);
        tutorialToggleQuestionmark.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //if rightInstantiate is not active, do something
        if(!rightInstantiate.activeSelf && !leftMenu.activeSelf && !rightMove.activeSelf && !rightSelect.activeSelf){
            if(!flipTutorialButton){
                //tutorialToggleQuestionmark.SetActive(true);
                flipTutorialButton = true;
            }
        }
    }

    public void DisableLeftMenuHint(){
        leftMenu.SetActive(false);
        isShowingTutorial = false;
    }

    public void DisableRightMoveHint(){
        rightMove.SetActive(false);
        isShowingTutorial = false;
    }

    public void DisableRightSelectHint(){
        rightSelect.SetActive(false);
        isShowingTutorial = false;
    }

    public void DisableRightInstantiateHint(){
        rightInstantiate.SetActive(false);
        isShowingTutorial = false;
    }

    void SetShowTutorial(bool value){
        isShowingTutorial = value;
        ActivateTutorial();
    }

    public void ToggleShowTutorial(){
        isShowingTutorial = !isShowingTutorial;
        flipTutorialButton = false;
        ActivateTutorial();
    }

    void ActivateTutorial(){
        //tutorialToggleQuestionmark.SetActive(!isShowingTutorial);
        leftMenu.SetActive(isShowingTutorial);
        rightMove.SetActive(isShowingTutorial);
        rightSelect.SetActive(isShowingTutorial);
        rightInstantiate.SetActive(isShowingTutorial);
    }
}
