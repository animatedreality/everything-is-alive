using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonaTest : MonoBehaviour
{
    private string url = "https://api.monaverse.com/public/tokens/1/contract/38/animation";

    private IEnumerator GetAnimationData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("accept", "application/json");

            // Send the request and wait for a response
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // Handle the response
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }

    public void StartRequest()
    {
        StartCoroutine(GetAnimationData());
    }
}
