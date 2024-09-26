using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using DG.Tweening;
using Oculus.Interaction;

public class Creature : MonoBehaviour
{
    [Header("Audio")]
    public Instrument instrument;
    public InstGroup instGroup;
    public float currentVolume;

    [Header("Settings")]
    public bool pokable = false;//only drum-like sequence based instruments can be poked
    public bool isSelected = false;
    public bool availableToMerge = false;
    private CreatureGroup creatureGroup;

    [Header("Creature Manipulation")]
    public Transform eventWrapperParent;
    List<PointableUnityEventWrapper> interactableEventWrappers = new List<PointableUnityEventWrapper>();

    [HideInInspector]
    public GameObject moveAnchor;
    Coroutine availableToMergeCoroutine;


    void Start()
    {
        if (instrument == null)
        {
            instrument = GetComponentInChildren<Instrument>();
        }
        //instrument.SetClip(clip);
        if (instGroup == null)
        {
            instGroup = GetComponentInChildren<InstGroup>();
            instGroup.creature = this;
        }
        Global.instance.OnEveryStepEvent += EveryStep;
        Global.instance.AddCreature(this);
        SetVolume(1);
        //set availableToMergeCoroutine
        availableToMergeCoroutine = StartCoroutine(SetAvailableToMergeTrueWithDelay());

        //set moveAnchor if hasn't been set
        if(moveAnchor == null){
            moveAnchor = Instantiate(Global.instance.moveAnchor, transform.position, Quaternion.identity, transform);
            moveAnchor.transform.localPosition = Vector3.zero;
            // if(moveAnchor.GetComponent<MoveAnchor>() == null){
            //     moveAnchor.AddComponent<MoveAnchor>();
            // }
            //moveAnchor.GetComponent<MoveAnchor>().onValueChanged.AddListener(OnMoveAnchorValueChanged);
        }
        moveAnchor.transform.localScale = Vector3.zero;

        //get all the pointableEventWrappers from all the children of eventWrapperParent
        interactableEventWrappers = new List<PointableUnityEventWrapper>(eventWrapperParent.GetComponentsInChildren<PointableUnityEventWrapper>());
        //automatically assign events to all the pointableEventWrappers
        foreach(PointableUnityEventWrapper eventWrapper in interactableEventWrappers){
            eventWrapper.WhenSelect.AddListener(OnSelected);
        }
        LookAtPlayer();
        //OnDeselected();
    }

    public void MakeAvailable()
    {
        StopCoroutine(availableToMergeCoroutine);
        availableToMergeCoroutine = StartCoroutine(SetAvailableToMergeTrueWithDelay());
    }

    IEnumerator SetAvailableToMergeTrueWithDelay()
    {
        //give it some time between groups
        yield return new WaitForSeconds(2f);
        availableToMerge = true;
    }

    public void Delete()
    {
        if (creatureGroup != null)
        {
            creatureGroup.RemoveCreature(this);
        }
        Global.instance.RemoveCreature(this);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        foreach (PointableUnityEventWrapper eventWrapper in interactableEventWrappers)
        {
            eventWrapper.WhenSelect.RemoveListener(OnSelected);
        }
    }

    void OnDestroy(){
        //remove self from Global.instance.creatures
        Global.instance.RemoveCreature(this);
        foreach (PointableUnityEventWrapper eventWrapper in interactableEventWrappers)
        {
            eventWrapper.WhenSelect.RemoveListener(OnSelected);
        }
        Global.instance.OnEveryStepEvent -= EveryStep;
    }

    public CreatureGroup GetGroup()
    {
        return creatureGroup;
    }

    public void SetGroup(CreatureGroup group)
    {
        this.creatureGroup = group;
    }

    public int GetLoopLength()
    {
        return instrument.GetLoopLength();
    }

    public void SetLoopLength(int _loopLength)
    {
        instrument.loopLength = _loopLength;
    }

    //--------PUBLIC FUNCTIONS--------
    //This is called whtn the creature is touched physically
    public void OnPoke()
    {
        if (!pokable) return;
        instrument.Play();
    }

    public void SetVolume(float value){
        if(instGroup == null) return;
        if(instGroup.instrumentType == InstrumentType.STAR){
            instGroup.creatureStars.SetVolume(value);
        }else{
            foreach(Instrument instrument in instGroup.instruments){
                instrument.SetVolume(value);
                Debug.Log("SetVolume of Instrument" + value);
            }
        }

        SetMaterialEffect(value == 0);
        SetAnimation(value != 0);
        currentVolume = value;
    }

    public void SetMaterialEffect(bool _Deactivate){
        Debug.Log("Deactivate");
        //Find all Materials in Children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers){
            foreach (Material material in renderer.materials) {
                material.SetInt("_Deactivate", _Deactivate ? 1 : 0);
            }
        }
    }

    void SetAnimation(bool _animate){
        Debug.Log("SetAnimation" + _animate);
        //Find all Animators in Children
        CreatureAnimation creatureAnimation = GetComponentInChildren<CreatureAnimation>();
        if(creatureAnimation != null){
            creatureAnimation.SetAnimation(_animate);
        }
    }


    //----------SELECTION DESELECTION----------
    private Coroutine deselectCoroutine;

    //Select Creature by hovering hand over, directly or from raycast
    //This is called when the creature is selected
    //enables MoveAnchor
    //enables Sequencer
    public void OnSelected(PointerEvent pointerEvent){
        Debug.Log("OnSelected" + gameObject.name);
        //if there is deselection happening, cancel it
        Global.instance.CancelDeselectCoroutine();

        if(Global.instance.currentSelectedCreature == this) return;
        //deselect last one
        if(Global.instance.currentSelectedCreature != null){
            Global.instance.currentSelectedCreature.OnDeselected();
        }
        


        if(instGroup != null){
            instGroup.SetVisuals(true);
        }
        // else{
        //     //Debug.LogError("Weird, instGroup is null" + gameObject.name);
        // }
        moveAnchor.SetActive(true);
        moveAnchor.transform.DOScale(1, 0.5f);

        //start animation

        //interrupt deselectCoroutine
        if(deselectCoroutine != null){
            StopCoroutine(deselectCoroutine);
            deselectCoroutine = null;
        }
        Global.instance.currentSelectedCreature = this;
        Debug.Log("ToggleVolumeUI" + gameObject.name);
        MenuManager.instance.ToggleVolumeUI(true, currentVolume);
        isSelected = true;
    }

    public void OnSelectedDebug(){
        //if there is deselection happening, cancel it
        Global.instance.CancelDeselectCoroutine();

        if(Global.instance.currentSelectedCreature == this) return;
        //deselect last one
        if(Global.instance.currentSelectedCreature != null){
            Global.instance.currentSelectedCreature.OnDeselected();
        }
        
        //enable sequencer & moveanchor
        Debug.Log("OnSelected" + gameObject.name);

        if(instGroup != null){
            instGroup.SetVisuals(true);
        }
        // }else{
        //     Debug.LogError("Weird, instGroup is null" + gameObject.name);
        // }
        moveAnchor.SetActive(true);
        moveAnchor.transform.DOScale(1, 0.5f);

        //start animation

        //interrupt deselectCoroutine
        if(deselectCoroutine != null){
            StopCoroutine(deselectCoroutine);
            deselectCoroutine = null;
        }
        Global.instance.currentSelectedCreature = this;
        MenuManager.instance.ToggleVolumeUI(true, currentVolume);
        isSelected = true;
    }


    public void OnDeselected(){
        Debug.Log("OnDeselected" + gameObject.name);
        //disable MoveAnchor
        moveAnchor.transform.DOScale(0, 0.5f).OnComplete(() => {
            moveAnchor.SetActive(false);
        });
        //disable Sequencer
        if(instGroup != null){
            instGroup.SetVisuals(false);
        }
        // else{
        //     Debug.LogError("Weird, instGroup is null");
        // }

        Global.instance.currentSelectedCreature = null;
        MenuManager.instance.ToggleVolumeUI(false, currentVolume);
        isSelected = false;
    }

    public void SetInstGroup(InstGroup instGroup)
    {
        this.instGroup = instGroup;
        instrument.SetInstGroup(instGroup);
    }

    void EveryStep(int globalBeatIndex)
    {
        if (globalBeatIndex % 4 == 0)
        {
            CheckProximityToCreateGroup();
        }
    }

    public void LookAtPlayer()
    {
        AudioListener audioListener = FindObjectOfType<AudioListener>();
        if (audioListener != null)
        {
            Vector3 dirToListener = audioListener.transform.position - transform.position;
            Vector3 horizontalDirection = Vector3.ProjectOnPlane(dirToListener, Vector3.up);
            Quaternion rotation = Quaternion.LookRotation(horizontalDirection);
            rotation *= Quaternion.Euler(0, 180, 0);
            transform.rotation = rotation;
            // Vector3 directionAway = transform.position - audioListener.transform.position;
            // transform.rotation = Quaternion.LookRotation(directionAway);
        }
    }


    void CheckProximityToCreateGroup()
    {
        if (!Global.instance.mergeCreatures) return;
        if (!availableToMerge) return;
        if (creatureGroup == null)
        {
            foreach (var otherCreature in Global.instance.GetCreatures())
            {
                if (!otherCreature.availableToMerge) continue;
                if (otherCreature != this && otherCreature.creatureGroup == null && Vector3.Distance(transform.position, otherCreature.transform.position) <= Global.instance.creatureUnifyDistance)
                {
                    CreateNewGroup(this, otherCreature);
                    break;
                }
            }
        }
    }

    void CreateNewGroup(Creature creature1, Creature creature2)
    {
        if (!availableToMerge) return;
        GameObject go = PrefabManager.instance.creatureGroupPrefab;
        Vector3 newPos = (creature1.transform.position + creature2.transform.position) / 2;
        //the instgroup prefab has a default height so it will be slightly above newPos when instantiated, you can adjust that height if you want
        CreatureGroup newGroup = Instantiate(go, newPos, Quaternion.identity).GetComponent<CreatureGroup>();
        newGroup.AddCreature(creature1);
        newGroup.AddCreature(creature2);
    }


    public void RemovedFromGroup()
    {
        availableToMerge = false;
        creatureGroup = null;
        instrument.FindOriginalInstGroup();
        MakeAvailable();
    }


}