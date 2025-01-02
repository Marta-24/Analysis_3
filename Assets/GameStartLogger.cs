using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameStartLogger : MonoBehaviour
{
    private string url = "https://citmalumnes.upc.es/~albertcf5/D3/save_game_start.php"; // Replace with your actual PHP script URL

    void Start()
    {
        // Example player ID and start time
        string playerId = "Player1"; // Replace with actual player identification logic
        string startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Start sending data to the server
        StartCoroutine(SendGameStartTime(playerId, startTime));
    }

    private IEnumerator SendGameStartTime(string playerId, string startTime)
    {
        // Create form data
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);
        form.AddField("start_time", startTime);

        // Send POST request to the server
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        // Check response from the server
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
