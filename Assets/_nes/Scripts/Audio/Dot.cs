using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Audio
{
    public class Dot : MonoBehaviour
    {
        public Sprite empty, filled;
        public bool isOn;
        public Image image;

        [HideInInspector]
        public Instrument instrument;

        public void SetState(bool state)
        {
            isOn = state;
            image.sprite = isOn ? filled : empty;
            int index = transform.GetSiblingIndex();
            if (isOn)
            {
                instrument.AddToSequence(index);
            }
            else
            {
                instrument.RemoveFromSequence(index);
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
}