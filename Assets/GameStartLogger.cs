using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameStartLogger : MonoBehaviour
{
    private string url = "https://citmalumnes.upc.es/~albertcf5/D3/save_game_start.php";

    void Start()
    {
        string playerId = "Player1";
        string startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        StartCoroutine(SendGameStartTime(playerId, startTime));
    }

    private IEnumerator SendGameStartTime(string playerId, string startTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);
        form.AddField("start_time", startTime);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
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
