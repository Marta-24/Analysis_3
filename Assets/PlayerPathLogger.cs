using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerPathLogger : MonoBehaviour
{
    private string logUrl = "https://citmalumnes.upc.es/~albertcf5/D3/save_player_path.php";
    private string sessionUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_last_session.php";
    private int sessionID = -1;

    void Start()
    {
        StartCoroutine(GetSessionID());
    }

    IEnumerator GetSessionID()
    {
        UnityWebRequest www = UnityWebRequest.Get(sessionUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            SessionResponse sessionResponse = JsonUtility.FromJson<SessionResponse>(jsonResponse);
            sessionID = sessionResponse.session_id;
            // Debug.Log("Fetched session ID: " + sessionID);
            InvokeRepeating(nameof(LogPlayerPositionRepeating), 1f, 1f);
        }
        else
        {
            Debug.LogError("Error fetching session ID: " + www.error);
        }
    }

    void LogPlayerPositionRepeating()
    {
        if (sessionID != -1)
        {
            Vector3 position = transform.position;
            // Debug.Log($"Logging position: {position}");
            LogPlayerPosition(sessionID, "Player1", position);
        }
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

        UnityWebRequest www = UnityWebRequest.Post(logUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log("Player path logged: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error logging player path: " + www.error);
        }
    }

    [System.Serializable]
    private class SessionResponse
    {
        public int session_id;
    }
}
