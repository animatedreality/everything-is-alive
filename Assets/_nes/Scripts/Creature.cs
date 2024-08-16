using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class Creature : MonoBehaviour
{
    public Instrument instrument;
    InstGroup instGroup;
    public AudioClip clip;

    private CreatureGroup creatureGroup;

    [HideInInspector]
    public bool availableToMerge = false;

    Coroutine availableToMergeCoroutine;

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
        Global.instance.AddCreature(this);
        //set availableToMergeCoroutine
        availableToMergeCoroutine = StartCoroutine(SetAvailableToMergeTrueWithDelay());
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

    public CreatureGroup GetGroup()
    {
        return creatureGroup;
    }

    public void SetGroup(CreatureGroup group)
    {
        this.creatureGroup = group;
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
