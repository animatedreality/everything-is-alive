using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace Audio
{
    public enum InstrumentType{
        SEQUENCER,
        SAMPLE,
        STAR
    }
    public class InstGroup : MonoBehaviour
    {
        public Creature creature;
        public InstrumentType instrumentType = InstrumentType.SEQUENCER;
        public List<Instrument> instruments = new List<Instrument>();
        public InstrumentSample instrumentSampleScript;
        [Header("Star")]
        public CreatureStars creatureStars;
        [HideInInspector]
        public string name, id;
        bool playRuleInitialized = false;//to avoid initializing play rule twice

        public int loopLength;

        public Slider playHeadSlider;
        private float sliderHeight;

        public RectTransform playHeadHandle, playHeadArea, canvasRect, canvasShoulderL, canvasShoulderR;

        public Transform pointerSurfaceTransform;

        public Transform instrumentsParent;

        public UnityEvent<int> OnEveryStep;

        public Canvas canvas;
        private Vector3 originalScale = new Vector3(0.05f, 0.05f, 0.1f);

        bool isActive = true;
        //if instrumentType is SAMPLE


        void Start()
        {
            this.id = Util.AutoID();

            if(instrumentType == InstrumentType.STAR){
                ToggleSequencerCanvas(false);
            }
            AdjustPlayHead();
            AdjustPointerSurface();
            Global.instance.AddInstGroup(this);
            //SetPlayRule(true);
            //LookAtPlayer();
        }

        // void LookAtPlayer()
        // {
        //     //find object with component AudioListener and lookat it.. point towards the player when instantiated
        //     AudioListener audioListener = FindObjectOfType<AudioListener>();
        //     if (audioListener != null)
        //     {
        //         Vector3 directionAway = transform.position - audioListener.transform.position;
        //         transform.rotation = Quaternion.LookRotation(directionAway);
        //     }
        // }

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
            if(instrumentType == InstrumentType.SEQUENCER){
                playHeadSlider.maxValue = loopLength - 1;
                playHeadSlider.value = 0;
            }else if(instrumentType == InstrumentType.SAMPLE){
                playHeadSlider.maxValue = 100f;
                playHeadSlider.value = 0;
            }
            float newWidth = canvasRect.rect.width / loopLength;
            playHeadHandle.sizeDelta = new Vector2(newWidth, playHeadHandle.sizeDelta.y);
            //set the playhead area left and right padding to be half
            float padding = newWidth / 2;
            playHeadArea.offsetMin = new Vector2(padding, playHeadArea.offsetMin.y);
            playHeadArea.offsetMax = new Vector2(-padding, playHeadArea.offsetMax.y);
            Debug.Log("AdjustPlayHead: " + playHeadSlider.maxValue);
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
        //is called in Instrument > ResetInstGroup > AddInstrument
        void SetIsActive(bool isActive)
        {
            Debug.Log("set is active: " + name + " " + isActive);
            this.isActive = isActive;
            canvas.gameObject.SetActive(isActive);
            if (isActive)
            {
                //LookAtPlayer();
                SetPlayRule(true);
                Global.instance.AddInstGroup(this);

            }
            else
            {
                SetPlayRule(false);
                Global.instance.RemoveInstGroup(this);
            }
        }

        private void SetPlayRule(bool _start){
            Debug.Log("set play rule start: " + " " + playRuleInitialized);
            if(_start && !playRuleInitialized){
                if(instrumentType == InstrumentType.SEQUENCER){
                    Global.instance.OnEveryStepEvent += EveryStep;
                }else if(instrumentType == InstrumentType.SAMPLE){
                    StartCoroutine(SetPlayHeadSliderwithSample());
                }
                playRuleInitialized = true;
            }
            if(!_start && playRuleInitialized){
                if(instrumentType == InstrumentType.SEQUENCER){
                    Global.instance.SafeUnsubscribeFromEveryStep(EveryStep);
                }else if (instrumentType == InstrumentType.SAMPLE){
                    StopCoroutine(SetPlayHeadSliderwithSample());
                }
                playRuleInitialized = false;
            }
            Debug.Log("set play rule done: " + " " + playRuleInitialized);
        }

        IEnumerator SetPlayHeadSliderwithSample(){
            while(true){
                float samplePlayTime = instrumentSampleScript.GetSamplePlayTime();
                Debug.Log("playing coroutine: " + samplePlayTime);
                // Convert the samplePlayTime (0 to 1) to the slider's range (0 to loopLength - 1)
                playHeadSlider.value = samplePlayTime * 100f;
                yield return new WaitForSeconds(0.2f);
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
            SetPlayRule(false);
            Global.instance.RemoveInstGroup(this);
        }

        void ResizeCanvas()
        {
            canvasRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x, instruments.Count * Global.instance.instrumentUIHeight);
            sliderHeight = 1 + (instruments.Count-1) * 2f;
            playHeadSlider.transform.localScale = new Vector3(
                playHeadSlider.transform.localScale.x,
                sliderHeight,
                playHeadSlider.transform.localScale.z
            );
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
            Vector3 targetScale = _show ? originalScale : new Vector3(0, 0, 0);
            transform.DOScale(targetScale, 0.35f);
            // canvas.gameObject.SetActive(_show);
            // if(canvasShoulderL != null)
            // canvasShoulderL.gameObject.SetActive(_show);
            // if(canvasShoulderR != null)
            // canvasShoulderR.gameObject.SetActive(_show);
        }

        void ToggleSequencerCanvas(bool _show){
            canvas.gameObject.SetActive(_show);
            pointerSurfaceTransform.gameObject.SetActive(_show);
        }

    }
}