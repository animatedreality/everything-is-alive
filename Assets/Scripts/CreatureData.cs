using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CreatureBase", menuName = "ScriptableObjects/CreatureBaseScriptableObject")]
public class CreatureData : ScriptableObject
{
    public string name;
    public GameObject prefab;
    public Sprite sprite;
    public List<AudioClip> audioClips;
    [Header("Usually CreatureMemeberCount = audioClips.Count")]
    public int creatureMemberCount = 1;
    public enum CreatureType {
        Drum,
        Melody,
        Pad,
        Stars
    }
    [Header("Creature Type determines Sequencer Type")]
    public CreatureType creatureType;
    [Header("Length of Sequence")]
    public int sequenceLengthMultiplier = 1;
    public int sequenceLength = 16;
}
