using UnityEngine;
using UnityEngine.Networking; // Required for UnityWebRequest
using System.Collections;    // Required for IEnumerator

public class DatabaseConnectionTest : MonoBehaviour
{
    private string url = "https://citmalumnes.upc.es/~albertcf5/D3/db_connect.php"; // Replace with your server's URL

    void Start()
    {
        StartCoroutine(TestConnection());
    }

    private IEnumerator TestConnection()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Debug.Log("Server Response: " + www.downloadHandler.text);
        }
    }
}
