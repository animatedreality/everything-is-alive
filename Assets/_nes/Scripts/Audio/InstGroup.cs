using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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

        public Transform pointerSurfaceTransform;

        public Transform instrumentsParent;

        public UnityEvent<int> OnEveryStep;

        public Canvas canvas;

        void Start()
        {
            this.id = Util.AutoID();
            AdjustPlayHead();
            AdjustPointerSurface();
            Global.instance.OnEveryStepEvent += EveryStep;
        }

        private void Reset()
        {
            if (OnEveryStep == null)
            {
                OnEveryStep = new UnityEvent<int>();
            }
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

        private void AdjustPointerSurface()
        {
            //set the scale of the pointer surface to match the canvas dimensions times the canvasRect scale
            pointerSurfaceTransform.localScale = new Vector3(canvasRect.rect.width * canvasRect.localScale.x, canvasRect.rect.height * canvasRect.localScale.y, 0.01f);
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

        public void EveryStep(int globalBeatIndex)
        {
            int localBeatIndex = globalBeatIndex % loopLength;
            playHeadSlider.value = localBeatIndex;
            OnEveryStep?.Invoke(localBeatIndex);
        }

        public void AddInstrument(Instrument instrument)
        {
            instrument.SetLoopLength(loopLength);
            instruments.Add(instrument);
            instrument.transform.SetParent(instrumentsParent);
            ResizeCanvas();
        }

        public void RemoveInstrument(Instrument instrument)
        {
            instruments.Remove(instrument);
            ResizeCanvas();
        }

        void OnDestroy()
        {
            Global.instance.OnEveryStepEvent -= EveryStep;
        }

        void ResizeCanvas()
        {
            canvasRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x, instruments.Count * 64);
            AdjustPointerSurface();
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