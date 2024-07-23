using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FO_InstrumentManager : MonoBehaviour{
    public static FO_InstrumentManager s { get; private set; }
    private void Awake()
    {
        if (s == null)
        {
            s = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (s != this)
        {
            Destroy(gameObject);
        }
    }
    [SerializeField]
    public FO_InstrumentPrefab[] instrumentPrefabs;
    public FO_InstrumentPrefab currentInstrumentPrefab;
    public float spawnRadius = 0.5f;
    public int spawnRangeMin = 2;
    public int spawnRangeMax = 5;

    // Start is called before the first frame update
    void Start()
    {
        currentInstrumentPrefab = instrumentPrefabs[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RandomizeCurrentInstrumentPrefab(){
        int randomIndex = Random.Range(0, instrumentPrefabs.Length);
        currentInstrumentPrefab = instrumentPrefabs[randomIndex];
    }

    public void SetCurrentInstrument(FO_InstrumentPrefab _prefab){
        currentInstrumentPrefab = _prefab;
    }

    public void SpawnCurrentInstrumentObjects(Vector3 _position)
    {
        currentInstrumentPrefab.SpawnInstrumentObjects(_position);

    }
}
