using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureStars : MonoBehaviour
{
    public Transform starContainer;
    public GameObject starConnectorPrefab;
    public Transform starConnectorContainer;
    List<GameObject> stars = new List<GameObject>();
    List<StarConnector> starConnectors = new List<StarConnector>();
    public GameObject slider;
    public float sliderTravelSpeed = 1f;
    public bool isPlaying = false;

    public int currentIndex = 0;
    Vector3 targetPosition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        //add all childobjects from starContainer to stars List
        foreach (Transform child in starContainer)
            if (child.gameObject.activeSelf) stars.Add(child.gameObject);

        GenerateConnectors();

        AnimateSlider();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateConnectors();
    }

    void PlaySound(int _idx){
        stars[_idx].GetComponent<AudioSource>().Play();
        if(stars[_idx].GetComponent<ScaleOnPlay>()){
            stars[_idx].GetComponent<ScaleOnPlay>().ScaleUp();
        }
    }

    void AnimateSlider(){
        if(stars.Count < 2) return;
        targetPosition = stars[currentIndex].transform.position;
        StartCoroutine(MoveSlider());
    }

    IEnumerator MoveSlider(){
        while(true)
        {
            slider.transform.position = Vector3.MoveTowards(slider.transform.position, targetPosition, sliderTravelSpeed * Time.deltaTime);
            // Check if we've reached the target
            if (Vector3.Distance(slider.transform.position, targetPosition) < 0.01f)
            {
                PlaySound(currentIndex);
                // Move to the next star
                currentIndex = (currentIndex + 1) % stars.Count;
                targetPosition = stars[currentIndex].transform.position;
            }
            yield return null;
        }
    }

    void GenerateConnectors()
    {
        for (int i = 0; i < stars.Count; i++)
        {
            if((i+1) == stars.Count) return;//if it's last star, ignore

            GameObject star1 = stars[i];
            GameObject star2 = stars[(i + 1)];

            // Create connector
            GameObject connector = Instantiate(starConnectorPrefab, star1.transform.position, Quaternion.identity);
            connector.transform.SetParent(starConnectorContainer); // Set parent to keep scene organized
            StarConnector starConnector = new StarConnector(connector, star1.transform, star2.transform);
            starConnectors.Add(starConnector);
        }
    }

    void UpdateConnectors(){
        foreach(StarConnector connector in starConnectors){
            connector.UpdatePosition();
        }
    }
}


public class StarConnector
{
    public GameObject connectorObject;
    public Transform star1;
    public Transform star2;

    public StarConnector(GameObject connector, Transform s1, Transform s2)
    {
        connectorObject = connector;
        star1 = s1;
        star2 = s2;
    }

    public void UpdatePosition(){
        Vector3 direction = star2.transform.position - star1.transform.position;
        float distance = direction.magnitude;
        connectorObject.transform.position = star1.transform.position;
        connectorObject.transform.LookAt(star2.transform);
        connectorObject.transform.Rotate(-90, 0, 0); // Adjust rotation to point along local Y-axis
        connectorObject.transform.localScale = new Vector3(connectorObject.transform.localScale.x, distance, connectorObject.transform.localScale.z);
    }
}