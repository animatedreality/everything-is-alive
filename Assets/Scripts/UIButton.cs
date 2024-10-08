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
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Initialize(Sprite _sprite, string _name, UIButtonContainer _container){
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(OnClick);
        image.sprite = _sprite;
        name = _name;
        container = _container;
        container.buttons.Add(this);
        UnselectSelf();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick(){
        SelectSelf();
    }

    public void SelectSelf(){
        image.color = UIManager.i.buttonSelectedColor;
        isSelected = true;
        container.SelectButton(this);
    }

    public void UnselectSelf(){
        image.color = UIManager.i.buttonUnselectedColor;
        isSelected = false;
    }
}
