using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureFamily : MonoBehaviour
{
    public CreatureData creatureData;
    public Transform manipulationUI;
    public Sequencer sequencer;
    public Transform meshContainer;

    public GameObject creatureMesh;
    public List<CreatureMember> creatureMembers;
    public bool isSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one * CreatureManager.i.creatureScaleMultiplier;
        Initialize(CreatureManager.i.selectedCreatureData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(CreatureData _data){
        creatureData = _data;
        //generate mesh
        creatureMesh = Instantiate(creatureData.prefab, meshContainer);
        creatureMesh.transform.localPosition = Vector3.zero;
        creatureMesh.transform.localRotation = Quaternion.identity;
        creatureMesh.transform.localScale = Vector3.one;
        //initialize creature members
        //find all the CreatureMember components in the creatureMesh
        creatureMembers = new List<CreatureMember>(creatureMesh.GetComponentsInChildren<CreatureMember>());
        //IMPORTANT: Always Initialize Sequencer before CreatureMembers
        sequencer.Initialize(this);
        //generate amount of sequencers based on amount of creature members
        //assign a sequence and main sequencer to each creature member
        if(creatureData.creatureType == CreatureData.CreatureType.Drum || creatureData.creatureType == CreatureData.CreatureType.Pad){
            if(creatureMembers.Count != sequencer.sequenceAmount){
                Debug.LogError("DRUM & PAD: CreatureMember count does not match sequencer sequence amount" + name);
                return;
            }
            for(int i = 0; i < creatureMembers.Count; i++){
                creatureMembers[i].sequencer = sequencer;
                if(creatureMembers[i] is CreatureMemberDefault){
                    CreatureMemberDefault member = (CreatureMemberDefault)creatureMembers[i];
                    member.Initialize(sequencer.sequences[i]);
                }

            }
        }else if(creatureData.creatureType == CreatureData.CreatureType.Stars){
            for(int i = 0; i < creatureMembers.Count; i++){
                if(creatureMembers[i] is CreatureMemberStar){
                    CreatureMemberStar member = (CreatureMemberStar)creatureMembers[i];
                    member.Initialize(this, i);
                }
            }
        }

        transform.name = creatureData.name;

        //look at player
        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        AudioListener audioListener = FindObjectOfType<AudioListener>();
        if (audioListener != null)
        {
            Vector3 dirToListener = audioListener.transform.position - transform.position;
            Vector3 horizontalDirection = Vector3.ProjectOnPlane(dirToListener, Vector3.up);
            Quaternion rotation = Quaternion.LookRotation(horizontalDirection);
            rotation *= Quaternion.Euler(0, 180, 0);
            transform.rotation = rotation;
        }
    }

    public void OnSelect(){
        if(isSelected){
            return;
        }
        isSelected = true;
        CreatureManager.i.SelectCreatureFamily(this);
        manipulationUI.GetComponent<H_AnimateAppearance>().AnimateIn();
    }

    public void OnDeselect(){
        if(!isSelected){
            return;
        }
        isSelected = false;
        if(CreatureManager.i.selectedCreatureFamily == this){
            CreatureManager.i.selectedCreatureFamily = null;
        }
        manipulationUI.GetComponent<H_AnimateAppearance>().AnimateOut();
    }

    public void DestroySelf(){
        OnDeselect();
        Destroy(gameObject);
    }

    public void ToggleMute(){
        sequencer.ToggleMute();
    }

    public void SetVolume(float _volume){
        sequencer.SetVolume(_volume);
    }
}
