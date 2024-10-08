using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager : MonoBehaviour
{
    public static CreatureManager i;
    private void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float creatureScaleMultiplier = 3;
    //Load all CreatureData from Resources/CreatureData
    public List<CreatureData> creatureDataList;
    public GameObject creatureFamilyPrefab;
    public Transform creatureSpawnPoint;

    //assigned when the creature's UIButtonContainer SelectButton() is called
    public CreatureData selectedCreatureData;

    // Start is called before the first frame update
    public void Initialize()
    {
        creatureDataList = new List<CreatureData>(Resources.LoadAll<CreatureData>("CreatureData"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCreature(){
        //Spawn Creature if there has been one selected
        if(selectedCreatureData != null){
            GameObject creature = Instantiate(creatureFamilyPrefab);
            creature.transform.position = creatureSpawnPoint.position;
            Debug.Log("SpawnCreature");
        }
    }
}
