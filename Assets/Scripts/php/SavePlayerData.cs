using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SavePlayerData : MonoBehaviour
{
    public string url = "http://127.0.0.1/edsa-asyncOprdracht/SavedData.php";
    public long PlayerID = 12345;
    public long TotalGold = 5000;

    void Start()
    {
        StartCoroutine(SaveData());
    }

    IEnumerator SaveData()
    {
        WWWForm form = new WWWForm();
        form.AddField("PlayerID", PlayerID.ToString());  
        form.AddField("TotalGold", TotalGold.ToString());  

        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Debug.Log("Data saved successfully!");
        }
    }
}
