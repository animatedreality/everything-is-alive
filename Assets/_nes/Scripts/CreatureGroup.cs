using System.Collections.Generic;
using UnityEngine;
using Audio;

public class CreatureGroup : MonoBehaviour
{
    private List<Creature> creatures = new List<Creature>();
    private float unifyDistance = 2.0f;

    public GameObject instGroupPrefab;

    void Start()
    {
        Global.instance.OnEveryStepEvent += EveryStep;
        //create an InstGroup and add it to the creature group as a child
        GameObject instGroupGO = Instantiate(instGroupPrefab, transform);
        InstGroup instGroup = instGroupGO.GetComponent<InstGroup>();
        //set loop length to first creature's loop length
        instGroup.loopLength = creatures[0].GetLoopLength();
        instGroup.name = "Group " + Util.AutoID();
        //for each creature in the group, set the instGroup
        foreach (var creature in creatures)
        {
            creature.SetInstGroup(instGroup);
        }
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
        foreach (var creature in FindObjectsOfType<Creature>())
        {
            if (creature.GetGroup() == null && IsCloseToGroup(creature))
            {
                AddCreature(creature);
            }
        }
    }

    bool IsCloseToGroup(Creature creature)
    {
        foreach (var member in creatures)
        {
            if (Vector3.Distance(member.transform.position, creature.transform.position) <= unifyDistance)
            {
                return true;
            }
        }
        return false;
    }

    public void AddCreature(Creature creature)
    {
        if (!creatures.Contains(creature))
        {
            creatures.Add(creature);
            creature.SetGroup(this);
        }
    }

    public void RemoveCreature(Creature creature)
    {
        if (creatures.Contains(creature))
        {
            creatures.Remove(creature);
            creature.RemovedFromGroup();
            if (creatures.Count == 1)
            {
                creatures[0].RemovedFromGroup();
                Destroy(gameObject);
            }
        }
    }
}
