using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class DataAPI
{
    public string baseUrl;

    // Constructor
    public DataAPI(string baseUrl)
    {
        this.baseUrl = baseUrl;
    }

    // Example: endpoint = "/users"
    public IEnumerator GetRequest(string endpoint, System.Action<User[]> onComplete)
    {
        string uri = baseUrl + endpoint;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError($"[DataAPI] Error: {webRequest.error}");
                    onComplete?.Invoke(null);
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError($"[DataAPI] HTTP Error: {webRequest.error}");
                    onComplete?.Invoke(null);
                    break;

                case UnityWebRequest.Result.Success:
                    string json = webRequest.downloadHandler.text;

                    try
                    {
                        UserResponse response = JsonUtility.FromJson<UserResponse>(json);
                        if (response != null && response.success)
                            onComplete?.Invoke(response.user);
                        else
                            onComplete?.Invoke(null);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("[DataAPI] JSON parse error: " + e);
                        onComplete?.Invoke(null);
                    }
                    break;
            }
        }
    }
}
