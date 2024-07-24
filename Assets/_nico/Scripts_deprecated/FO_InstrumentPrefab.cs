using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FO_InstrumentPrefab : MonoBehaviour
{
    public GameObject[] instrumentObjects;
    public GameObject instrumentSelectorUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnInstrumentObjects(Vector3 _pos){
        //instantiate a random object from instrumentObjects at give position _pos
        int randomIndex = Random.Range(0, instrumentObjects.Length);
        GameObject instrument = Instantiate(instrumentObjects[randomIndex], _pos, Quaternion.identity);

        
        //SPAWNING IN BUNDLES
        // Debug.Log("Spawning instruments around " + _position);
        // int spawnCount = Random.Range(spawnRangeMin, spawnRangeMax);

        // for (int i = 0; i < spawnCount; i++)
        // {
        //     float randomX = Random.Range(-spawnRadius, spawnRadius);
        //     float randomZ = Random.Range(-spawnRadius, spawnRadius);
        //     Vector3 spawnPosition = new Vector3(_position.x + randomX, _position.y, _position.z + randomZ);

        //     int randomIndex = Random.Range(0, instrumentPrefabs.Length);
        //     GameObject instrument = Instantiate(instrumentPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        // }
    }

}
