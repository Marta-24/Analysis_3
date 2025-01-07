using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;

public class PauseMenuLogger : MonoBehaviour
{
    private string pauseLogUrl = "https://citmalumnes.upc.es/~albertcf5/D3/save_pause_data.php";  // URL to save pause data
    private string sessionUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_last_session.php";  // URL to fetch the last session ID
    private int sessionID = -1;  // Default invalid session ID
    private string playerId = "Player1";  // Replace with actual player ID

    void Start()
    {
        StartCoroutine(GetSessionID());
    }

    private IEnumerator GetSessionID()
    {
        UnityWebRequest www = UnityWebRequest.Get(sessionUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            SessionResponse response = JsonUtility.FromJson<SessionResponse>(www.downloadHandler.text);
            sessionID = response.session_id;
            Debug.Log($"Fetched session ID: {sessionID}");
        }
        else
        {
            Debug.LogError("Error fetching session ID: " + www.error);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LogPauseEvent();
        }
    }

    private void LogPauseEvent()
    {
        Vector3 playerPosition = transform.position;  // Get player's current position
        string pauseTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        Debug.Log($"Pause Menu Opened: \n" +
                  $"Session ID: {sessionID}\n" +
                  $"Player ID: {playerId}\n" +
                  $"Pause Time: {pauseTime}\n" +
                  $"Position: (X: {playerPosition.x:F4}, Y: {playerPosition.y:F4}, Z: {playerPosition.z:F4})");

        StartCoroutine(SendPauseData(pauseTime, playerPosition));
    }

    private IEnumerator SendPauseData(string pauseTime, Vector3 position)
    {
        if (sessionID == -1)
        {
            Debug.LogError("No valid session ID. Cannot log pause menu event.");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("session_id", sessionID);
        form.AddField("player_id", playerId);
        form.AddField("pause_time", pauseTime);
        form.AddField("position_x", position.x.ToString("F4", CultureInfo.InvariantCulture));
        form.AddField("position_y", position.y.ToString("F4", CultureInfo.InvariantCulture));
        form.AddField("position_z", position.z.ToString("F4", CultureInfo.InvariantCulture));

        UnityWebRequest www = UnityWebRequest.Post(pauseLogUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Pause menu event logged successfully!");
        }
        else
        {
            Debug.LogError("Failed to log pause menu event: " + www.error);
        }
    }

    [System.Serializable]
    public class SessionResponse
    {
        public int session_id;
    }
}
