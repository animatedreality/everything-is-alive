using System.Collections.Generic;
using UnityEngine;
using Audio;

public class CreatureGroup : MonoBehaviour
{
    public List<Creature> creatures = new List<Creature>();

    public InstGroup instGroup;

    void Awake()
    {
        //delete all instruments since the prefab has one instrument by default
        instGroup.DeleteAllInstruments();
        instGroup.name = "Group " + Util.AutoID();
    }
    void Start()
    {
        InitiateDefaultCreatures();
        Global.instance.OnEveryStepEvent += EveryStep;
    }

    void EveryStep(int globalBeatIndex)
    {
        if (globalBeatIndex % 4 == 0)
        {
            CheckProximity();
        }
    }

    void OnDestroy()
    {
        Global.instance.OnEveryStepEvent -= EveryStep;
    }

    void CheckProximity()
    {
        List<Creature> creaturesToRemove = new List<Creature>();
        List<Creature> creaturesToAdd = new List<Creature>();

        foreach (var creature in creatures)
        {
            if (creature.GetGroup() == this)
            {
                if (!IsCloseToGroup(creature))
                {
                    creaturesToRemove.Add(creature);
                }
            }

        }
        foreach (var creature in Global.instance.GetCreatures())
        {
            if (creature.availableToMerge)
            {
                if (creature.GetGroup() != this && IsCloseToGroup(creature))
                {
                    creaturesToAdd.Add(creature);
                }
            }
        }

        // Remove creatures after iterating
        foreach (var creature in creaturesToRemove)
        {
            RemoveCreature(creature);
        }

        // Add creatures after iterating
        foreach (var creature in creaturesToAdd)
        {
            AddCreature(creature);
        }
    }


    bool IsCloseToGroup(Creature creature)
    {
        float distance = Vector3.Distance(transform.position, creature.transform.position);
        if (distance <= Global.instance.creatureUnifyDistance)
        {
            return true;
        }
        return false;
    }

    public void AddCreature(Creature creature)
    {
        if (!creature.availableToMerge)
        {
            return;
        }
        if (creatures.Count == 0)
        {
            //set loop length to first creature's loop length
            instGroup.loopLength = creature.GetLoopLength();
        }
        if (!creatures.Contains(creature))
        {
            creature.availableToMerge = false;
            creature.SetGroup(this);
            creature.SetInstGroup(instGroup);
            creatures.Add(creature);
        }

    }

    public void RemoveCreature(Creature creature)
    {
        creatures.Remove(creature);
        creature.RemovedFromGroup();
        if (creatures.Count == 1)
        {
            creatures[0].RemovedFromGroup();
            creatures.Clear();
            Destroy(gameObject);
        }
    }
    //when the creatures are assigned at start, initialize them
    private void InitiateDefaultCreatures(){
        if(creatures.Count == 0) return;
        foreach (Creature creature in creatures){
            creature.SetGroup(this);
            creature.SetInstGroup(instGroup);
            creature.SetLoopLength(instGroup.loopLength);
        }
        
    }
}
