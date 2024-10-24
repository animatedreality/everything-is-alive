using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMemberStar : CreatureMember
{
    //TOO MUCH TO DO RIGHT NOW
    //DEAL WITH THIS LATER

    
    public AudioSource audioSource;
    public CreatureFamily creatureFamily;

    // [Header("Star Specific Function")]
    // int lastPositionIndex;
    // Coroutine positionCheckCoroutine;
    public void Initialize(CreatureFamily _creatureFamily, int _index){
        // creatureFamily = _creatureFamily;
        // audioSource = GetComponent<AudioSource>();
        // audioSource.clip = creatureFamily.creatureData.audioClips[_index];
        // Debug.Log("Initialized CreatureMemberStar" + gameObject.name);

        // //Check Position to Change Pitch
        // lastPositionIndex = CheckPositionIndex();
        // positionCheckCoroutine = StartCoroutine(CheckPosition());
    }

    // private IEnumerator CheckPosition(){
    //     while(true){
    //         if (lastPositionIndex != CheckPositionIndex())
    //         {
    //             UpdateStar();
    //             Debug.Log("Position changed to " + CheckPositionIndex());
    //             lastPositionIndex = CheckPositionIndex();
    //         }
    //         yield return new WaitForSeconds(0.1f);
    //     }
    // }

    // private int CheckPositionIndex(){
    //     return Mathf.Clamp(Mathf.FloorToInt(transform.localPosition.y / 0.4f), 0, creatureStars.clips.Length - 1);
    // }

    // private void UpdateStar(){
    //     audioSource.clip = creatureFamily.creatureData.audioClips[CheckPositionIndex()];
    // }

}
