using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TokenVerifycation : MonoBehaviour
{
    public IEnumerator PerformActionWithToken(string actionUrl, Action onSuccess, Action onFailure)
    {
        string token = PlayerPrefs.GetString("UserToken");

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("No token found. Please log in first.");
            onFailure?.Invoke();
            yield break;
        }

        using (UnityWebRequest webRequest = UnityWebRequest.Get(actionUrl))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Token validation failed: " + webRequest.error);
                onFailure?.Invoke();
            }
            else
            {
                Debug.Log("Token validation successful.");
                onSuccess?.Invoke();
            }
        }
    }
}
