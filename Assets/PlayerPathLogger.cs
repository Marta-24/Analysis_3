using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerPathLogger : MonoBehaviour
{
    private string url = "https://citmalumnes.upc.es/~albertcf5/D3/save_player_path.php";

    void Start()
    {
        // Start logging player position every second
        InvokeRepeating(nameof(LogPlayerPositionRepeating), 1f, 1f);
    }

    void LogPlayerPositionRepeating()
    {
        Vector3 position = transform.position;
        Debug.Log($"Logging position: {position}");
        LogPlayerPosition(1, "Player1", position);
    }

    public void LogPlayerPosition(int sessionID, string playerName, Vector3 position)
    {
        StartCoroutine(SendPlayerPathData(sessionID, playerName, position));
    }

    private IEnumerator SendPlayerPathData(int sessionID, string playerName, Vector3 position)
    {
        WWWForm form = new WWWForm();
        form.AddField("session_id", sessionID);
        form.AddField("player_name", playerName);
        form.AddField("position_x", position.x.ToString("F4"));
        form.AddField("position_y", position.y.ToString("F4"));
        form.AddField("position_z", position.z.ToString("F4"));

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Player path logged: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error logging player path: " + www.error);
        }
    }
}
