using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//require Button component
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class UIButton : MonoBehaviour
{
    public bool isSelected = false;
    public UIButtonContainer container;
    Button button;
    Image image;
    Color unselectedColor, selectedColor;
    public CreatureData creatureData;

    public void Initialize(CreatureData _creatureData, UIButtonContainer _container){
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(OnClick);
        image.sprite = _creatureData.sprite;
        creatureData = _creatureData;
        container = _container;
        container.buttons.Add(this);
        UnselectSelf();
    }

    public void OnClick(){
        Debug.Log($"Button clicked for creature: {creatureData.name}");
        SelectSelf();

        //Verify the creature data was set
        if (CreatureManager.i.selectedCreatureData == creatureData)
        {
            Debug.Log($"Successfully selected creature: {creatureData.name}");
        }
        else {
            Debug.LogError("Failed to set selected creature data!");
        }
    }

    public void SelectSelf(){
        image.color = UIManager.i.buttonSelectedColor;
        isSelected = true;
        container.SelectButton(this);

        Debug.Log($"Selected creature: {creatureData.name}");
    }

    public void UnselectSelf(){
        image.color = UIManager.i.buttonUnselectedColor;
        isSelected = false;
    }
}
