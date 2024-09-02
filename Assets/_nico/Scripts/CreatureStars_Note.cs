using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class CreatureStars_Note : MonoBehaviour
{
    public CreatureStars creatureStars;
    public AudioSource audioSource;
    int lastPositionIndex;
    Coroutine positionCheckCoroutine;

    public void OnInitialize(CreatureStars _creatureStars){
        creatureStars = _creatureStars;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        lastPositionIndex = CheckPositionIndex();
        positionCheckCoroutine = StartCoroutine(CheckPosition());
        UpdateStar();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CheckPosition(){
        while(true){
            if (lastPositionIndex != CheckPositionIndex())
            {
                UpdateStar();
                Debug.Log("Position changed to " + CheckPositionIndex());
                lastPositionIndex = CheckPositionIndex();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private int CheckPositionIndex(){
        return Mathf.Clamp(Mathf.FloorToInt(transform.localPosition.y / creatureStars.pitchChangeAreaHeight), 0, creatureStars.clips.Length - 1);
    }

    public void SetMaterial(Color _color){
        Debug.Log("Setting material color to " + _color);
        //change alpha of this color to 0.2f or 0.2 * 255 whichever is correct
        _color.a = 0.2f;
        GetComponent<Renderer>().material.SetColor("_Color", _color);
    }

    private void UpdateStar(){
        audioSource.clip = creatureStars.clips[CheckPositionIndex()];
        SetMaterial(creatureStars.colors[CheckPositionIndex()]);
    }

    public void OnPlay(){
        audioSource.Play();
        if(GetComponent<ScaleOnPlay>()){
            GetComponent<ScaleOnPlay>().ScaleUp();
        }
        //Tween the star's transparency to 0 then back to original transparency
        GetComponent<Renderer>().material.DOFade(1, 0.15f).SetEase(Ease.InOutSine).OnComplete(() => GetComponent<Renderer>().material.DOFade(0.2f, 0.4f).SetEase(Ease.InOutSine));
    }

    public void SetVolume(float _volume){
        if(audioSource == null) return;
        audioSource.volume = _volume;
    }
}
