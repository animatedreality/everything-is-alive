using Meta.XR.MRUtilityKit;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
public class EA_XRData : MonoBehaviour
{
    public static EA_XRData s { get; private set; }
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

    //------MRUK ROOM------
    //public Transform mrRoomPrefab;
    [SerializeField]
    public List<MRUKAnchor> mrukAnchors = new List<MRUKAnchor>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetMRUKRoom(){
        // var room = MRUK.Instance.GetCurrentRoom();
        // Debug.Log("room.GetType() = " + room.GetType());
        // if (room != null)
        // {
        //     foreach (MRUKAnchor anchorPrefab in room.Anchors)
        //     {
        //         Debug.Log("anchor.name = " + anchorPrefab.name);
        //         Debug.Log("type = " + anchorPrefab.GetType());
        //         mrukAnchors.Add(anchorPrefab);
        //     }
        // }
        
    }

    
}
