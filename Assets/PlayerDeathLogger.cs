using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerDeathLogger : MonoBehaviour
{
    private string url = "https://citmalumnes.upc.es/~albertcf5/D3/SendPlayerDead.php";  // Replace with your actual PHP URL

    public void LogPlayerDeath(int sessionID, string playerName, string cause, Vector3 position)
    {
        StartCoroutine(SendPlayerDeathData(sessionID, playerName, cause, position));
    }

    private IEnumerator SendPlayerDeathData(int sessionID, string playerName, string cause, Vector3 position)
    {
        WWWForm form = new WWWForm();
        form.AddField("session_id", sessionID.ToString());
        form.AddField("player_name", playerName);
        form.AddField("cause", cause);
        form.AddField("x", position.x.ToString());
        form.AddField("y", position.y.ToString());
        form.AddField("z", position.z.ToString());

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Player death logged: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error logging player death: " + www.error);
        }
    }
}
