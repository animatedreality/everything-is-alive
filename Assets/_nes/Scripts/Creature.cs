using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class Creature : MonoBehaviour
{
    public Instrument instrument;
    InstGroup instGroup;
    public AudioClip clip;

    private CreatureGroup group;
    private float unifyDistance = 2.0f;

    void Start()
    {
        if (instrument == null)
        {
            instrument = GetComponentInChildren<Instrument>();
        }
        instrument.SetClip(clip);
        if (instGroup == null)
        {
            instGroup = GetComponentInChildren<InstGroup>();
        }
        Global.instance.OnEveryStepEvent += EveryStep;
    }

    public void Delete()
    {
        if (group != null)
        {
            group.RemoveCreature(this);
        }
        Destroy(gameObject);
    }

    public CreatureGroup GetGroup()
    {
        return group;
    }

    public void SetGroup(CreatureGroup group)
    {
        this.group = group;
    }



    void OnDestroy()
    {
        Global.instance.OnEveryStepEvent -= EveryStep;
    }

    public int GetLoopLength()
    {
        return instrument.GetLoopLength();
    }

    public void OnPoke()
    {
        instrument.Play();
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


    void CheckProximityToCreateGroup()
    {
        if (group == null)
        {
            foreach (var otherCreature in FindObjectsOfType<Creature>())
            {
                if (otherCreature != this && otherCreature.group == null && Vector3.Distance(transform.position, otherCreature.transform.position) <= unifyDistance)
                {
                    CreateNewGroup(this, otherCreature);
                    break;
                }
            }
        }
    }

    void CreateNewGroup(Creature creature1, Creature creature2)
    {
        GameObject go = PrefabManager.instance.creatureGroupPrefab;
        Vector3 newPos = (creature1.transform.position + creature2.transform.position) / 2;
        CreatureGroup newGroup = Instantiate(go, newPos, Quaternion.identity).GetComponent<CreatureGroup>();
        newGroup.AddCreature(creature1);
        newGroup.AddCreature(creature2);
    }

    public void RemovedFromGroup()
    {
        group = null;
    }


}
