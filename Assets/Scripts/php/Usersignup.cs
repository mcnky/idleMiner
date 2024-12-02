using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Usersignup : MonoBehaviour
{
    public TMP_InputField EmailInput;
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    public TMP_Text YouSignedUp;

    private void Start()
    {
        YouSignedUp.enabled = false;
    }

    public void SignUp()
    {
        CreateAccountRequest newAccount = new CreateAccountRequest
        {
            email = EmailInput.text,
            username = UsernameInput.text,
            password = PasswordInput.text
        };

        StartCoroutine(CreateAccountRequestAsync(newAccount));
    }

    private IEnumerator CreateAccountRequestAsync(CreateAccountRequest request)
    {
        string json = JsonUtility.ToJson(request);
        List<IMultipartFormSection> form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("data", json)
        };

        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://127.0.0.1/edsa-asyncOprdracht/Response.php", form))
        {
            webRequest.timeout = 10;
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError($"Request failed: {webRequest.error}");
                YouSignedUp.enabled = true;
            }
            else
            {
                if (webRequest.downloadHandler != null && !string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    try
                    {
                        CreateAccountResponse response = JsonUtility.FromJson<CreateAccountResponse>(webRequest.downloadHandler.text);
                        Debug.Log($"Account created: {response.status}, {response.customMessage}");

                        SceneManager.LoadScene("SampleScene");
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Raw response: " + webRequest.downloadHandler.text);
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

    [System.Serializable]
    public class CreateAccountRequest
    {
        public string action = "createAccount";
        public string email;
        public string username;
        public string password;
    }

    [System.Serializable]
    public class CreateAccountResponse
    {
        public string status;
        public string customMessage;
    }
}
