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

        public RectTransform playHeadHandle, playHeadArea, canvasRect, canvasShoulderL, canvasShoulderR;

        public Transform pointerSurfaceTransform;

        public Transform instrumentsParent;

        public UnityEvent<int> OnEveryStep;

        public Canvas canvas;

        bool isActive = true;

        void Start()
        {
            this.id = Util.AutoID();
            AdjustPlayHead();
            AdjustPointerSurface();
            Global.instance.AddInstGroup(this);
            Global.instance.OnEveryStepEvent += EveryStep;
            LookAtPlayer();
        }

        void LookAtPlayer()
        {
            //find object with component AudioListener and lookat it.. point towards the player when instantiated
            AudioListener audioListener = FindObjectOfType<AudioListener>();
            if (audioListener != null)
            {
                Vector3 directionAway = transform.position - audioListener.transform.position;
                transform.rotation = Quaternion.LookRotation(directionAway);
            }
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
            if (!isActive) return;
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
            if (!isActive) return;
            int localBeatIndex = globalBeatIndex % loopLength;
            playHeadSlider.value = localBeatIndex;
            OnEveryStep?.Invoke(localBeatIndex);
        }

        public void AddInstrument(Instrument instrument)
        {
            if (!instruments.Contains(instrument))
            {
                instrument.SetLoopLength(loopLength);
                instruments.Add(instrument);
                instrument.transform.SetParent(instrumentsParent);
                //set instruments z position to 0 and scale to 1,1,1 and rotation to 0,0,0
                instrument.transform.localPosition = new Vector3(0, instrument.transform.localPosition.y, 0);
                instrument.transform.localScale = new Vector3(1, 1, 1);
                instrument.transform.localRotation = Quaternion.identity;
                ResizeCanvas();
            }
            //show canvas
            if (instruments.Count > 0)
            {
                SetIsActive(true);
            }
        }

        void SetIsActive(bool isActive)
        {
            this.isActive = isActive;
            canvas.gameObject.SetActive(isActive);
            if (isActive)
            {
                LookAtPlayer();
                Global.instance.OnEveryStepEvent += EveryStep;
                Global.instance.AddInstGroup(this);
            }
            else
            {
                Global.instance.OnEveryStepEvent -= EveryStep;
                Global.instance.RemoveInstGroup(this);
            }
        }

        public void RemoveInstrument(Instrument instrument)
        {
            instruments.Remove(instrument);
            ResizeCanvas();
            if (instruments.Count == 0)
            {
                //hide canvas
                SetIsActive(false);
            }
        }

        public void DeleteAllInstruments()
        {
            //for each sibling of the instrumentsParent, destroy it
            foreach (Transform child in instrumentsParent)
            {
                Destroy(child.gameObject);
            }
            instruments.Clear();
            SetIsActive(false);
            ResizeCanvas();
        }

        void OnDestroy()
        {
            Global.instance.OnEveryStepEvent -= EveryStep;
        }

        void ResizeCanvas()
        {
            canvasRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x, instruments.Count * Global.instance.instrumentUIHeight);
            AdjustPointerSurface();
        }

        public void Schedule(int globalBeatIndex, double nextEventTime)
        {
            if (!isActive) return;
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

        public void SetVisuals(bool _show)
        {
            canvas.gameObject.SetActive(_show);
            canvasShoulderL.gameObject.SetActive(_show);
            canvasShoulderR.gameObject.SetActive(_show);
        }

    }
}