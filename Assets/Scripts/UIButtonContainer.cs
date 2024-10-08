using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonContainer : MonoBehaviour
{
    public List<UIButton> buttons;
    public UIButton selectedButton;
    // Start is called before the first frame update
    public void Initialize(List<CreatureData> _creatureDataList){
        foreach(CreatureData creatureData in _creatureDataList){
            GameObject button = Instantiate(UIManager.i.buttonPrefab, transform);
            button.GetComponent<UIButton>().Initialize(creatureData, this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectButton(UIButton _button){
        if(selectedButton != null && selectedButton != _button)
            selectedButton.UnselectSelf();
        selectedButton = _button;

        //if I am defaultCreatureUIButtonContainer in UIManager, assign the selectedCreature to selectedCreatureData in CreatureManager
        if(this == UIManager.i.defaultCreatureUIButtonContainer){
            CreatureManager.i.selectedCreatureData = selectedButton.creatureData;
        }
    }
}
