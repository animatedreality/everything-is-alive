using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreatureStars : MonoBehaviour
{
    [Header("Objects")]
    public List<CreatureStars_Note> starNotes = new List<CreatureStars_Note>();
    public Transform starContainer;
    public GameObject starConnectorPrefab;
    public Transform starConnectorContainer;
    List<StarConnector> starConnectors = new List<StarConnector>();
    public GameObject slider;
    public float sliderTravelSpeed = 1f;
    public bool isPlaying = false;

    public int currentIndex = 0;
    Vector3 targetPosition = Vector3.zero;
    [Header("Audio Clips from low to high")]
    public AudioClip[] clips;
    public Color[] colors;//make sure colors are the same length as clips
    public float pitchChangeAreaHeight = 0.4f;//height of area where pitches are the same

    // Start is called before the first frame update
    void Start()
    {
        GenerateConnectors();
        foreach(CreatureStars_Note note in starNotes)
            note.OnInitialize(this);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateConnectors();
        AnimateSlider();
    }

    void PlaySound(int _idx){
        starNotes[_idx].OnPlay();
    }

    void AnimateSlider(){
        if(starNotes.Count < 2) return;
        targetPosition = starNotes[currentIndex].transform.position;
        slider.transform.position = Vector3.MoveTowards(slider.transform.position, targetPosition, sliderTravelSpeed * Time.deltaTime);
                // Check if we've reached the target
        if (Vector3.Distance(slider.transform.position, targetPosition) < 0.01f)
        {
            PlaySound(currentIndex);
            // Move to the next star
            currentIndex = (currentIndex + 1) % starNotes.Count;
            targetPosition = starNotes[currentIndex].transform.position;
        }
    }

    void GenerateConnectors()
    {
        for (int i = 0; i < starNotes.Count; i++)
        {
            if((i+1) == starNotes.Count) return;//if it's last star, ignore

            GameObject star1 = starNotes[i].gameObject;
            GameObject star2 = starNotes[(i + 1)].gameObject;

            // Create connector
            GameObject connector = Instantiate(starConnectorPrefab, star1.transform.position, Quaternion.identity);
            connector.transform.SetParent(starConnectorContainer); // Set parent to keep scene organized
            StarConnector starConnector = new StarConnector(connector, star1.transform, star2.transform, transform.localScale.x * 1.04f);
            starConnectors.Add(starConnector);
        }
    }

    void UpdateConnectors(){
        foreach(StarConnector connector in starConnectors){
            connector.UpdatePosition();
        }
    }

    public void SetVolume(float _volume){
        foreach(CreatureStars_Note note in starNotes){
            note.SetVolume(_volume);
        }
    }
}


public class StarConnector
{
    public GameObject connectorObject;
    public Transform star1;
    public Transform star2;
    public float scaleMultipler;

    public StarConnector(GameObject connector, Transform s1, Transform s2, float s)
    {
        connectorObject = connector;
        star1 = s1;
        star2 = s2;
        scaleMultipler = s;
    }

    public void UpdatePosition(){
        Vector3 direction = star2.transform.position - star1.transform.position;
        float distance = direction.magnitude / scaleMultipler;
        connectorObject.transform.position = star1.transform.position;
        connectorObject.transform.LookAt(star2.transform);
        connectorObject.transform.Rotate(-90, 0, 0); // Adjust rotation to point along local Y-axis
        connectorObject.transform.localScale = new Vector3(connectorObject.transform.localScale.x, distance, connectorObject.transform.localScale.z);
    }
}