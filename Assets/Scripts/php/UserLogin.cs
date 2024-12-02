using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections.Generic;

public class UserLogin : MonoBehaviour
{
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    public TMP_Text LoginStatusText;

    private void Start()
    {
        LoginStatusText.enabled = false;
    }

    public void LogIn()
    {
        LoginRequest loginRequest = new LoginRequest
        {
            username = UsernameInput.text,
            password = PasswordInput.text
        };

        StartCoroutine(LoginRequestAsync(loginRequest));
    }

    private IEnumerator LoginRequestAsync(LoginRequest request)
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
                Debug.LogError($"Login request failed: {webRequest.error}");
                LoginStatusText.text = "Login failed!";
                LoginStatusText.enabled = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    try
                    {
                        LoginResponse response = JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);
                        if (response.status == "success")
                        {
                            Debug.Log("Login successful, token received: " + response.token);

                            // Save token locally for future requests
                            PlayerPrefs.SetString("UserToken", response.token);
                            LoginStatusText.text = "Login successful!";
                            LoginStatusText.enabled = true;

                            // Load the next scene
                            Debug.Log("Login was correct. Moving to the next scene.");
                            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                            SceneManager.LoadScene(currentSceneIndex + 1);  // Load next scene
                        }
                        else
                        {
                            LoginStatusText.text = "Login failed!";
                            LoginStatusText.enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Failed to parse response: " + ex.Message);
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
    public class LoginRequest
    {
        public string action = "login";
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string status;
        public string token;
    }
}
