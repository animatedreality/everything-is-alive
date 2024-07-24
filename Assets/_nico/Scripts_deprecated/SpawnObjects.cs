using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    public GameObject[] prefabs;
    public float spawnDiameter = 1.5f;
    public float spawnDistanceMin = 0.2f;//how close are objects within each others
    public int spawnAmount = 10;
    public AudioClip[] audioClips;
    public Color[] pedalColors;

    void Start()
    {
        //SpawnObjectsFunc();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnObjectsFromFOSpawnPositions(){
        List<Vector3> spawnPositions = FO_FindSpawnPositions.s.spawnPositions;
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            GameObject spawnedObject = Instantiate(prefabs[Random.Range(0, prefabs.Length)], spawnPositions[i], Quaternion.identity);
            spawnedObject.transform.parent = transform;
            InitiateInstrument(spawnedObject, i);
        }
    }

    //this function is DEPRECATED because we're getting positions from FO_FindSpawnPositions
    void SpawnObjectsFunc(){
        List<Vector3> usedPositions = new List<Vector3>();
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomCircle = Random.insideUnitCircle.normalized * spawnDiameter;
            Vector3 spawnDirection = new Vector3(randomCircle.x, 0, randomCircle.y);
            Vector3 spawnPoint = transform.position + spawnDirection;

            // Check if the spawnPoint is too close to any already used positions
            bool isTooClose = false;
            foreach (Vector3 usedPosition in usedPositions)
            {
                if (Vector3.Distance(spawnPoint, usedPosition) < spawnDistanceMin)
                {
                    isTooClose = true;
                    break;
                }
            }

            if (!isTooClose)
            {
                GameObject spawnedObject = Instantiate(prefabs[Random.Range(0, prefabs.Length)], spawnPoint, Quaternion.identity);
                spawnedObject.transform.parent = transform;
                InitiateInstrument(spawnedObject, i);
                usedPositions.Add(spawnPoint);
            }
            else
            {
                i--; // Decrement i to retry this iteration with a new position
            }
        }
    }

    void InitiateInstrument(GameObject _prefab, int _idx){
        // if prefab has audiosource, then assign a random audioClip from audioClip array
        if (_prefab.GetComponent<AudioSource>() != null)
        {
            //assign audioClips based on _idx, if _idx exceeds audioClips.Length, start from 0 again
            _prefab.GetComponent<AudioSource>().clip = audioClips[_idx % audioClips.Length];
        }

        if(_prefab.GetComponent<SpawnedObject>() != null){
            _prefab.GetComponent<SpawnedObject>().InitializeSpawnedObject(pedalColors[Random.Range(0, pedalColors.Length)], 0);
        }
    }
}
