using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;

public class InteractionTrigger : MonoBehaviour
{
    public string interactionType;
    private string interactionLogUrl = "https://citmalumnes.upc.es/~albertcf5/D3/save_interaction_data.php";
    private string sessionUrl = "https://citmalumnes.upc.es/~albertcf5/D3/get_last_session.php";
    private int sessionID = -1;
    private string playerId = "Player1";

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
            // Debug.Log($"Fetched session ID: {sessionID}");
        }
        else
        {
            Debug.LogError("Error fetching session ID: " + www.error);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LogInteraction();
        }
    }

    private void LogInteraction()
    {
        Vector3 playerPosition = transform.position;
        string interactionTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        /*
        Debug.Log($"Interaction Logged: \n" +
        $"Session ID: {sessionID}\n" +
        $"Player ID: {playerId}\n" +
        $"Interaction Type: {interactionType}\n" +
        $"Interaction Time: {interactionTime}\n" +
        $"Position: (X: {playerPosition.x:F4}, Y: {playerPosition.y:F4}, Z: {playerPosition.z:F4})");
        */
        StartCoroutine(SendInteractionData(interactionType, interactionTime, playerPosition));
    }

    private IEnumerator SendInteractionData(string interactionType, string interactionTime, Vector3 position)
    {
        if (sessionID == -1)
        {
            Debug.LogError("No valid session ID. Cannot log interaction.");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("session_id", sessionID);
        form.AddField("player_id", playerId);
        form.AddField("interaction_type", interactionType);
        form.AddField("interaction_time", interactionTime);
        form.AddField("position_x", position.x.ToString("F4", CultureInfo.InvariantCulture));
        form.AddField("position_y", position.y.ToString("F4", CultureInfo.InvariantCulture));
        form.AddField("position_z", position.z.ToString("F4", CultureInfo.InvariantCulture));

        UnityWebRequest www = UnityWebRequest.Post(interactionLogUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log("Interaction logged successfully!");
        }
        else
        {
            Debug.LogError("Failed to log interaction: " + www.error);
        }
    }

    [System.Serializable]
    public class SessionResponse
    {
        public int session_id;
    }
}