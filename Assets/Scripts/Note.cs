using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Note : MonoBehaviour
{
    public Sprite empty, filled;
    public bool isOn;
    public Image image;

    [HideInInspector]
    public Sequence sequence;

    public void Initialize(Sequence _sequence){
        sequence = _sequence;
        sequence.notes.Add(this);
    }

    public void SetState(bool state)
    {
        isOn = state;
        image.sprite = isOn ? filled : empty;
        int index = transform.GetSiblingIndex();
        if (isOn)
        {
            sequence.AddToSequencePattern(index);
        }
        else
        {
            sequence.RemoveFromSequencePattern(index);
        }
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        SetState(!isOn);
    }

    public void TurnOn()
    {
        SetState(true);
    }

    public void TurnOff()
    {
        SetState(false);
    }
}
