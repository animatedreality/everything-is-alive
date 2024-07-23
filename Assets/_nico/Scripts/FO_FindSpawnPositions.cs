using Meta.XR.MRUtilityKit;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;


public class FO_FindSpawnPositions : FindSpawnPositions
{
    public static FO_FindSpawnPositions s { get; private set; }
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

    private Transform cameraTransform;
    public UnityEvent onObjectsSpawned = new UnityEvent();

    //[SerializeField] private GameObject spawnLocationIndicator;
    //[SerializeField] private int spawnTimer;

    public System.Collections.Generic.List<Vector3> spawnPositions = new System.Collections.Generic.List<Vector3>();


    
    private void Start()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
    }


    public void SpawnPositions()
    {
        Debug.Log("Span Position Called");
        var room = MRUK.Instance.GetCurrentRoom();
        if(room != null)
            Debug.Log("ROOM WAS SUCCESSFULLY GOTTEN");
        var minRadius = 0.2f;

        for (int i = 0; i < SpawnAmount; ++i)
        {
            var randomPos = room.GenerateRandomPositionInRoom(minRadius, true);
            Vector3 spawnPosition = Vector3.zero;
            if (SpawnLocations == SpawnLocation.Floating)
            {
                
                if (!randomPos.HasValue)
                {
                    break;
                }

                spawnPosition = randomPos.Value;
            }
            else
            {
                MRUK.SurfaceType surfaceType = 0;
                switch (SpawnLocations)
                {
                    case SpawnLocation.AnySurface:
                        surfaceType |= MRUK.SurfaceType.FACING_UP;
                        surfaceType |= MRUK.SurfaceType.VERTICAL;
                        surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                        break;
                    case SpawnLocation.VerticalSurfaces:
                        surfaceType |= MRUK.SurfaceType.VERTICAL;
                        break;
                    case SpawnLocation.OnTopOfSurfaces:
                        surfaceType |= MRUK.SurfaceType.FACING_UP;
                        break;
                    case SpawnLocation.HangingDown:
                        surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                        break;
                }
                if (room.GenerateRandomPositionOnSurface(surfaceType, minRadius, LabelFilter.FromEnum(Labels), out var pos, out var normal))
                {
                    spawnPosition = pos;
                }
            }

            spawnPositions.Add(spawnPosition);
        }

        //finish spawning, call event onObjectsSpawned
        onObjectsSpawned?.Invoke();
    }

}
