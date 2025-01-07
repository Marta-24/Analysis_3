using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerDeathLogger : MonoBehaviour
{
    private string deathLogUrl = "https://citmalumnes.upc.es/~albertcf5/D3/SendPlayerDead.php";  // URL to log player death
    private string sessionUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_last_session.php";  // URL to get last session ID
    private int sessionID = -1;  // Default invalid session ID

    void Start()
    {
        StartCoroutine(GetLastSessionID());  // Fetch session ID when the game starts
    }

    private IEnumerator GetLastSessionID()
    {
        UnityWebRequest www = UnityWebRequest.Get(sessionUrl);  // Fetch last session ID from database
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Raw server response: {www.downloadHandler.text}");
            SessionResponse response = JsonUtility.FromJson<SessionResponse>(www.downloadHandler.text);
            sessionID = response.session_id;
            Debug.Log($"Fetched last session ID: {sessionID}");
        }
        else
        {
            Debug.LogError("Error fetching session ID: " + www.error);
        }
    }

    public void LogPlayerDeath(string playerName, string cause, Vector3 position)
    {
        if (sessionID == -1)
        {
            Debug.LogError("No valid session ID. Cannot log player death.");
            return;
        }

        StartCoroutine(SendPlayerDeathData(sessionID, playerName, cause, position));
    }

    private IEnumerator SendPlayerDeathData(int sessionID, string playerName, string cause, Vector3 position)
    {
        WWWForm form = new WWWForm();
        form.AddField("session_id", sessionID.ToString());
        form.AddField("player_name", playerName);
        form.AddField("cause", cause);
        form.AddField("x", position.x.ToString("F4"));
        form.AddField("y", position.y.ToString("F4"));
        form.AddField("z", position.z.ToString("F4"));

        UnityWebRequest www = UnityWebRequest.Post(deathLogUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Player death logged successfully: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error logging player death: " + www.error);
        }
    }

    [System.Serializable]
    public class SessionResponse
    {
        public int session_id;
    }
}
