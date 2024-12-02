using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JSONrequest : MonoBehaviour
{
    private string url = "http://127.0.0.1/edsa-asyncOprdracht/Response.php";

    private IEnumerator CreateAccountRequestAsync(CreateAccountRequest request)
    {
        string json = JsonUtility.ToJson(request);
        List<IMultipartFormSection> form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("data", json)
        };

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            webRequest.timeout = 10;
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError($"Request failed: {webRequest.error}");
            }
            else
            {
                if (webRequest.downloadHandler != null && !string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    try
                    {
                        CreateAccountResponse response = JsonUtility.FromJson<CreateAccountResponse>(webRequest.downloadHandler.text);
                        Debug.Log($"Account created: {response.status}, {response.customMessage}");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to parse response: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogError("Received an empty or null response");
                }
            }
        }
    }
}

[System.Serializable]
public class CreateAccountRequest
{
    public string action = "createAccount";
    public string email;
    public string username;
    public string password;
}

[System.Serializable]
public class LoginAccountRequest
{
    public string action = "loginAccount";
    public string email;
    public string password;
}

[System.Serializable]
public class CreateAccountResponse
{
    public string status;
    public string customMessage;
}

[System.Serializable]
public class LoginAccountResponse
{
    public string status;
    public string customMessage;
    public string token;
}
