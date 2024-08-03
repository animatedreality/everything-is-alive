using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Audio
{
    public class InstGroup : MonoBehaviour
    {
        [HideInInspector]
        public List<Instrument> instruments = new List<Instrument>();

        [HideInInspector]
        public string name, id;

        public int loopLength;

        public Slider playHeadSlider;

        public RectTransform playHeadHandle, playHeadArea, canvasRect;

        public Canvas canvas;

        void Start()
        {
            this.id = Util.AutoID();
            AdjustPlayHead();
        }

        private void AdjustPlayHead()
        {
            playHeadSlider.maxValue = loopLength - 1;
            playHeadSlider.value = 0;
            float newWidth = canvasRect.rect.width / loopLength;
            playHeadHandle.sizeDelta = new Vector2(newWidth, playHeadHandle.sizeDelta.y);
            //set the playhead area left and right padding to be half
            float padding = newWidth / 2;
            playHeadArea.offsetMin = new Vector2(padding, playHeadArea.offsetMin.y);
            playHeadArea.offsetMax = new Vector2(-padding, playHeadArea.offsetMax.y);
        }

        public void SetLoopLength(int loopLength)
        {
            this.loopLength = loopLength;
            foreach (Instrument instrument in instruments)
            {
                instrument.SetLoopLength(loopLength);
            }
            AdjustPlayHead();
        }

        public void EveryStepAction(int globalBeatIndex)
        {
            playHeadSlider.value = globalBeatIndex % loopLength;
        }

        public void AddInstrument(Instrument instrument)
        {
            instrument.SetLoopLength(loopLength);
            instruments.Add(instrument);
            ResizeCanvas();
        }

        public void RemoveInstrument(Instrument instrument)
        {
            instruments.Remove(instrument);
            ResizeCanvas();
        }

        void ResizeCanvas()
        {
            canvasRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x, instruments.Count * 64);
        }

        public void Schedule(int globalBeatIndex, double nextEventTime)
        {
            foreach (Instrument instrument in instruments)
            {
                instrument.Schedule(globalBeatIndex, nextEventTime);
            }
        }

        public void Play()
        {
            foreach (Instrument instrument in instruments)
            {
                instrument.Play();
            }
        }

        public void Stop()
        {
            foreach (Instrument instrument in instruments)
            {
                instrument.Stop();
            }
        }

    }
}